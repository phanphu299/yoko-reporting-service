using System.Collections.Generic;
using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class RunFailedSchedule : IRequest<IEnumerable<GenerateReportDto>>
    {
        public string JobId { get; set; }

        public IEnumerable<int> Templates { get; set; }
    }
}