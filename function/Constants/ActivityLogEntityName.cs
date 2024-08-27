namespace Reporting.Function.Constant
{
    public class ActivityLogEntityName
    {
        public const string REPORT = "Report";

        public static string GetActivityLogEntityName(string objectType)
        {
            return objectType switch
            {
                IOEntityType.REPORT => ActivityLogEntityName.REPORT,
                IOEntityType.SCHEDULE => ActivityLogEntityName.REPORT,
                IOEntityType.TEMPLATE => ActivityLogEntityName.REPORT,

                _ => null
            };
        }
    }
}