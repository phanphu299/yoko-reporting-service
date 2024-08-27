using System.IO;
using System.Data;

namespace Reporting.Application.Service.Abstraction
{
    public interface IAmDataTableBuilder
    {
        IAmDataTableBuilder BuildDataArray();
        DataTable BuildDataTable();
        void SetTimeZoneName(string timeZoneName);
        void SetDateTimeFormat(string dateTimeFormat);
        void SetDataSetContent(DataSetAmMappingContent dataSetContent);
        void SetRequestContent(RequestAmContent requestContent);
        void SetResponseContent(byte[] responseContent);
    }
}