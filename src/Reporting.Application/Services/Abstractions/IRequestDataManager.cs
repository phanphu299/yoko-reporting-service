namespace Reporting.Application.Service.Abstraction
{
    public interface IRequestDataManager
    {
        string EnrichSqlQuery(string url, long fromDate, long toDate);
        string EnrichRequestUrl(string url, long fromDate, long toDate);
    }
}