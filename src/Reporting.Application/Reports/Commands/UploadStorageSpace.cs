using System;
using System.IO;
using Reporting.Application.Extension;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class UploadStorageSpace : IRequest<UploadStorageSpaceDto>
    {
        public string StorageTypeId { get; set; }
        public string StorageContent { get; set; }
        public string OutputExtension { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string Name { get; set; }
        public Stream File { get; set; }
        public DateTime CreatedUtc { get; set; }

        public UploadStorageSpace(SimpleTemplateByIdDto template, Stream file, string timeZoneName, DateTime createdReportFileUTC, Guid? executionId = null)
        {
            StorageTypeId = template.Storage.TypeId;
            StorageContent = template.Storage.Content;
            OutputExtension = template.OutputType.Extension;
            File = file;
            CreatedUtc = DateTime.UtcNow;
            FolderName = $"reports/{(executionId.HasValue ? executionId.Value.ToString("N") : CreatedUtc.ToString(DateTimeExtension.SHORT_TIMESTAMP_FORMAT))}";
            FileName = $"{template.Name.ReplaceNonLetterOrDigit()}_{createdReportFileUTC.ToLocalDateTime(timeZoneName).ToString(DateTimeExtension.LONG_TIMESTAMP_FORMAT)}{OutputExtension}";
        }
    }
}