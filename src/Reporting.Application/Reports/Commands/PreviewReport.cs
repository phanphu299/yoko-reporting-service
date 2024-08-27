using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class PreviewReport : PreviewReportBase, IRequest<PreviewReportFileDto>
    {
    }
}