using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using Reporting.Application.Models;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Command
{
    public class ExportReportBySchedule : IRequest<ActivityResponse>
    {
        public Guid ActivityId { get; set; } = Guid.NewGuid();
        public string ObjectType { get; set; }
        public IEnumerable<string> ScheduleNames { get; set; }
        public string TemplateId { get; set; }
        public IEnumerable<string> ScheduleIds { get; set; }
    }
}