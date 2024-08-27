using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Dapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Reporting.Function.Constant;
using Reporting.Function.Extension;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using Reporting.Function.Exception;
using AHI.Infrastructure.SharedKernel.Abstraction;

namespace Reporting.Function.Service
{
    public class TemplateExportHandler : BaseExportHandler<ReportDto>, IExportHandler
    {
        private const string MODEL_NAME = "Template";
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly INativeStorageService _storageService;
        private readonly IDictionary<string, IStorageSpaceHandler> _storageSpaceHandler;
        private readonly IParserContext _parserContext;
        private readonly ILoggerAdapter<TemplateExportHandler> _logger;

        public TemplateExportHandler(IConfiguration configuration, ITenantContext tenantContext,
                                           INativeStorageService storageService, IParserContext parserContext,
                                            ILoggerAdapter<TemplateExportHandler> logger,
                                           IDictionary<string, IStorageSpaceHandler> storageSpaceHandler) : base(parserContext, storageSpaceHandler)
        {
            _tenantContext = tenantContext;
            _storageService = storageService;
            _configuration = configuration;
            _storageSpaceHandler = storageSpaceHandler;
            _parserContext = parserContext;
            _logger = logger;
        }

        public async Task<string> HandleAsync(ExecutionContext context, object data)
        {
            if (data == null)
            {
                throw ExceptionHelper.GenerateEntityInvalidException(ErrorCode.ERROR_ENTITY_INVALID, payload: data);
            }

            if (ExportData == null)
            {
                await GetDataAsync((DownloadReportRequest)data);
            }
            _parserContext.SetContextFormat(ContextFormatKey.LOG_ID, ExportData.Select(x => x.Id.ToString()).CombineData());
            _parserContext.SetContextFormat(ContextFormatKey.LOG_NAME, ExportData.Select(x => x.Name).CombineData());
            return await HandleUploadContentAsync(ExportData);
        }

        public async Task<int> GetDataAsync(DownloadReportRequest request)
        {
            var query = string.Empty;
            try
            {
                IEnumerable<ReportDto> reports = Array.Empty<ReportDto>();
                query = @"select  r.id as Id,
							r.name as Name,
                            r.file_name as FileName,
                            r.template_id as TemplateId,
                            tpl.name as TemplateName,
                            r.schedule_id as ScheduleId,
                            r.schedule_name as ScheduleName,
                            r.storage_url as StorageUrl,
                            s.content as StorageContent,
                            st.id as StorageType
                        from reports as r with(nolock)
                            join storages as s  with(nolock) on s.id = r.storage_id
                            join storage_types as st with(nolock)  on st.id = s.type_id
                            left join schedules as sch  with(nolock) on sch.id = r.schedule_id
                            left join templates as tpl with(nolock)  on tpl.id = r.template_id
                        where r.template_id in @TemplateIds";
                var connectionString = _configuration["ConnectionStrings:Default"].BuildConnectionString(_configuration, _tenantContext.ProjectId);
                using (var dbConnection = new SqlConnection(connectionString))
                {
                    reports = await dbConnection.QueryAsync<ReportDto>(query, new { TemplateIds = request.Ids }, commandTimeout: 600);
                    await dbConnection.CloseAsync();
                }

                if (!reports.Any())
                {
                    throw ExceptionHelper.GenerateEntityNotFoundException(ErrorCode.NO_REPORT_MATCH, payload: request);
                }
                ExportData = reports;
                return reports.Count();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, query);
                throw ExceptionHelper.GenerateCommonException(ExceptionErrorCode.ERROR_GENERIC_COMMON_EXCEPTION, ErrorCode.FAILED_WHILE_CONNECTING_TO_DB,
                        request, ex);
            }
        }

        private async Task<string> HandleUploadContentAsync(IEnumerable<ReportDto> reports)
        {
            if (reports.Count() == 1)
            {
                // return await DownloadSingleFileAsync(reports);
                // #65145 - refactor storage service: for now, AzureBlobStorageSpaceHandler is unused. So just return report file path should be enough
                return reports.First().StorageUrl;
            }

            using (var stream = new MemoryStream())
            {
                // download file and zip => return zip file
                string fileName = GetZipName();
                await CreateMultipleFileContentAsync(reports, stream);
                ExportData = null;
                stream.Seek(0, SeekOrigin.Begin);
                return await _storageService.UploadAsync(StorageConstants.DefaultExportPath, fileName, stream);
            }
        }

        // create zip content without writing temp file
        private async Task CreateMultipleFileContentAsync(IEnumerable<ReportDto> reports, Stream contentStream)
        {
            using (var zipArchive = new ZipArchive(contentStream, ZipArchiveMode.Create, true))
            {
                var hasTemplateFolder = false;
                if (reports.GroupBy(x => x.TemplateId).Count() > 1)
                {
                    hasTemplateFolder = true;
                }
                foreach (var r in reports)
                {
                    var scheduleFolderWithFileName = string.Concat(r.ScheduleName.ReplaceNonLetterOrDigit(), "/", r.FileName);
                    if (hasTemplateFolder)
                        scheduleFolderWithFileName = string.Concat(r.TemplateName.ReplaceNonLetterOrDigit(), "/", scheduleFolderWithFileName);

                    var newEntry = zipArchive.CreateEntry(scheduleFolderWithFileName);
                    using (var entryStream = newEntry.Open())
                    {
                        await _storageSpaceHandler[r.StorageType].DownloadFileAsync(r.StorageUrl, r.StorageContent, entryStream);
                    }
                }
            }
        }
    }
}