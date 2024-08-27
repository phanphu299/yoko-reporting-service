using AHI.Infrastructure.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IReportService : ISearchService<Domain.Entity.Report, int, SearchReport, ReportDto>
    {
        Task<ReportDetailDto> GetReportByIdAsync(GetReport command);

        Task<IEnumerable<DataTableResult>> GetDataSourceAsync(GetDataSource command);

        Task<UploadStorageSpaceDto> UploadStorageSpaceAsync(UploadStorageSpace command);

        Task<DownloadFileUrlDto> GetDownloadFileUrlAsync(GetDownloadFileUrl command);

        Task InitGenerateReportAsync(GenerateReport command);

        Task<IEnumerable<GenerateReportDto>> GenerateReportAsync(GenerateReport command);

        Task<ActivityResponse> DownloadReportsAsync(ExportReport command);

        Task<ActivityResponse> DownLoadReportsByTemplateAsync(ExportReportByTemplate command);

        Task<ActivityResponse> DownLoadReportsByScheduleAsync(ExportReportBySchedule command);

        Task<PreviewReportFileDto> PreviewReportAsync(PreviewReport command);

        Task<List<string>> GetReportIdsByTemplateIdsAsync(IEnumerable<string> templateIds);

        Task<List<string>> GetReportIdsByScheduleIdsAsync(IEnumerable<string> scheduleIds);

        Task<ActivityResponse> DownloadPreviewReportsAsync(ExportPreviewReport command);

        Task<string> CreatePreviewReportAsync(PreviewReportBase command, TemplateByIdDto template, string timeZoneName);
    }
}