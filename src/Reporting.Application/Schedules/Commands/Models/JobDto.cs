using System;
using System.Collections.Generic;

namespace Reporting.Application.Command.Model
{
    public class JobDto
    {
        public string Id { get; set; }
        public string Cron { get; set; }
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string TimeZoneName { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public IDictionary<string, object> AdditionalParams { get; set; }

        public JobDto()
        {
        }

        public JobDto(string id, string cron, string endpoint, string method, string timeZoneName, DateTime? start, DateTime? end, IDictionary<string, object> additionalParams)
        {
            Id = id;
            Cron = cron;
            Endpoint = endpoint;
            Method = method;
            TimeZoneName = timeZoneName;
            Start = start;
            End = end;
            AdditionalParams = additionalParams;
        }
    }
}