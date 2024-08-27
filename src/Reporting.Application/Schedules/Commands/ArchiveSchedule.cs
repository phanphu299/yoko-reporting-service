using MediatR;
using Reporting.Application.Command.Model;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Schedule.Command
{
    public class ArchiveSchedule : IRequest<IEnumerable<ScheduleDto>>
    {
        public DateTime ArchiveTime { get; set; } = DateTime.UtcNow;
    }
}
