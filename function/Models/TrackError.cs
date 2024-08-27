using Newtonsoft.Json;
using Reporting.Function.Constant;

namespace Reporting.Function.Model
{
    public class TrackError
    {
        [JsonProperty(Order = -2)]
        public string Type { get; set; }

        public string Message { get; set; }

        public TrackError(string message, ErrorType errorType = ErrorType.UNDEFINED)
        {
            Message = message;
            Type = errorType.ToString();
        }
    }
}