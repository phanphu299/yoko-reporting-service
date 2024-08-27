using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Service.Enum;
using Newtonsoft.Json.Linq;

namespace Reporting.Application.Helper
{
    public class AdvanceSearchHelper
    {
        public static JObject CreateCriteria(string queryKey, PageSearchType queryType, string operation, object queryValue)
        {
            if (string.IsNullOrEmpty(queryKey))
                throw new EntityInvalidException(nameof(queryKey));
            if (string.IsNullOrEmpty(operation))
                throw new EntityInvalidException(nameof(operation));
            var criteriaObject = JObject.FromObject(new
            {
                QueryKey = queryKey,
                QueryType = queryType,
                Operation = operation,
                QueryValue = queryValue
            });
            return criteriaObject;
        }

        public static JObject CreateAndCriteria(JObject[] criterias)
        {
            var criteriaObject = JObject.FromObject(new
            {
                And = criterias
            });
            return criteriaObject;
        }

        public static JObject CreateOrCriteria(JObject[] criterias)
        {
            var criteriaObject = JObject.FromObject(new
            {
                Or = criterias
            });
            return criteriaObject;
        }
    }
}