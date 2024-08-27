using MediatR;
using Reporting.Application.Models;

namespace Reporting.Application.Command
{
    public class ExportPreviewReport : PreviewReportBase, IRequest<ActivityResponse>
    {
    }
}