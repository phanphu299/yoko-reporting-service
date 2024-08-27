using System.IO;
using System.Collections.Generic;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class GetDataSource : IRequest<IEnumerable<DataTableResult>>
    {
        public Stream TemplateFile { get; set; }
        public string TimeZoneName { get; set; }
        public string DateTimeFormat { get; set; }
        public long FromDateUtcInMilliseconds { get; set; }
        public long ToDateUtcInMilliseconds { get; set; }
        public IEnumerable<DataSourceContentDto> DataSets { get; set; }
        public string CurrentDataSourceContent { get; set; }
        public string CurrentDataSetName { get; set; }

        public GetDataSource(Stream templateFile, string timeZoneName, string dateTimeFormat, long fromDateUtcInMilliseconds, long toDateUtcInMilliseconds, IEnumerable<DataSourceContentDto> dataSets)
        {
            TemplateFile = templateFile;
            TimeZoneName = timeZoneName;
            DateTimeFormat = dateTimeFormat;
            FromDateUtcInMilliseconds = fromDateUtcInMilliseconds;
            ToDateUtcInMilliseconds = toDateUtcInMilliseconds;
            DataSets = dataSets;
        }
    }
}