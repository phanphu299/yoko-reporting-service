using MediatR;
using Reporting.Application.Models;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Command
{
    public class ExportReportByTemplate : IRequest<ActivityResponse>
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string ObjectType { get; set; }
        public IEnumerable<string> TemplateIds { get; set; }
    }
}