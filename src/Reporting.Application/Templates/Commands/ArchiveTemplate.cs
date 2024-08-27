using MediatR;
using Reporting.Application.Command.Model;
using System;
using System.Collections.Generic;

namespace Reporting.Application.Template.Command
{
    public class ArchiveTemplate : IRequest<IEnumerable<TemplateBasicDto>>
    {
        public DateTime ArchiveTime { get; set; } = DateTime.UtcNow;
    }
}
