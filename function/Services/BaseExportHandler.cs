using Reporting.Function.Constant;
using Reporting.Function.Extension;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Function.Service
{
    public class BaseExportHandler<T>
    {
        private readonly IParserContext _parserContext;
        private readonly IDictionary<string, IStorageSpaceHandler> _storageSpaceHandler;
        protected IEnumerable<T> ExportData { get; set; }

        public BaseExportHandler(IParserContext parserContext,
                                 IDictionary<string, IStorageSpaceHandler> storageSpaceHandler)
        {
            _parserContext = parserContext;
            _storageSpaceHandler = storageSpaceHandler;
        }

        public virtual string GetZipName()
        {
            var timezoneOffset = GetTimeZoneInfo();
            return $"Report_{DateTime.UtcNow.ToTimestamp(timezoneOffset)}.zip";
        }

        public Task<string> DownloadSingleFileAsync(IEnumerable<ReportDto> reports)
        {
            var report = reports.First();
            return _storageSpaceHandler[report.StorageType].GetDownloadFileUrlAsync(report.StorageUrl, report.StorageContent);
        }

        private string GetTimeZoneInfo()
        {
            var timezoneOffset = _parserContext.GetContextFormat(ContextFormatKey.DATETIMEOFFSET) ?? DateTimeExtension.DEFAULT_DATETIME_OFFSET;
            return DateTimeExtension.ToValidOffset(timezoneOffset);
        }
    }
}