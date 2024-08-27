using Microsoft.Extensions.Configuration;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using Reporting.Domain.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class ReportNotifyContentBuilder : IReportNotifyContentBuilder
    {
        private readonly INativeStorageService _storageService;
        private readonly string _rootFilePath;

        public ReportNotifyContentBuilder(INativeStorageService storageService, IConfiguration configuration)
        {
            _storageService = storageService;
            _rootFilePath = configuration["FileDirectory"] ?? "/var/storage/files";
        }

        public async Task<string> CreateZipReportsAsync(ScheduleExecution execution, string timeZoneName, List<Report> reports)
        {
            var timestamp = $"{DateTime.UtcNow.ToLocalDateTime(timeZoneName).ToString(DateTimeExtension.LONG_TIMESTAMP_FORMAT)}";
            var fileName = $"Report_{timestamp}.zip";
            var zipFilePath = Path.Combine(_rootFilePath, "temp", fileName);

            if (!reports.Any())
                return string.Empty;

            foreach (var report in reports)
                await DownloadToZipFileAsync(zipFilePath, report.FileName, report.StorageUrl);

            var folderPath = $"reports/{execution.Id:N}";
            var path = await UploadZipFileToCdn(zipFilePath, folderPath, fileName);

            File.Delete(zipFilePath);

            return path;
        }

        private async Task DownloadToZipFileAsync(string zipFilePath, string reportName, string reportPath)
        {
            var directory = Path.GetDirectoryName(zipFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            FileMode fileMode;
            ZipArchiveMode zipMode;
            if (File.Exists(zipFilePath))
            {
                fileMode = FileMode.Open;
                zipMode = ZipArchiveMode.Update;
            }
            else
            {
                fileMode = FileMode.Create;
                zipMode = ZipArchiveMode.Create;
            }

            using (var file = File.Open(zipFilePath, fileMode, FileAccess.ReadWrite, FileShare.None))
            using (var zip = new ZipArchive(file, zipMode))
            {
                var entry = zip.CreateEntry(reportName);
                using (var stream = entry.Open())
                {
                    await _storageService.DownloadFileToStreamAsync(reportPath, stream);
                }
            }
        }

        private async Task<string> UploadZipFileToCdn(string zipFilePath, string folderPath, string fileName)
        {
            using (var file = File.OpenRead(zipFilePath))
            {
                var path = await _storageService.UploadCdnAsync(fileName, folderPath, file);
                return path;
            }
        }
    }
}