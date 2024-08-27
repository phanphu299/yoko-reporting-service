using Reporting.Application.Models;
using System;

namespace Reporting.Application.Command.Model
{
    public class PreviewReportFileDto : ActivityResponse
    {
        public string PreviewKey { get; set; }
        public string Endpoint { get; set; }

        public PreviewReportFileDto(Guid activityId, string previewKey, string endpoint)
            : base(activityId)
        {
            PreviewKey = previewKey;
            Endpoint = endpoint;
        }
    }
}