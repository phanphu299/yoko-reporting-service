using System.Text.RegularExpressions;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class RequestDataManager : IRequestDataManager
    {

        public string EnrichSqlQuery(string url, long fromDate, long toDate)
        {
            url = Regex.Replace(url, $"@{FieldName.FROM_DATE}", fromDate.ToString(), RegexOptions.IgnoreCase);
            url = Regex.Replace(url, $"@{FieldName.TO_DATE}", toDate.ToString(), RegexOptions.IgnoreCase);
            return url;
        }

        public string EnrichRequestUrl(string url, long fromDate, long toDate)
        {
            url = Regex.Replace(url, $"@{FieldName.FROM_DATE}", fromDate.ToString(), RegexOptions.IgnoreCase);
            url = Regex.Replace(url, $"@{FieldName.TO_DATE}", toDate.ToString(), RegexOptions.IgnoreCase);
            return url;
        }
    }
}