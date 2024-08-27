using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetReport : IRequest<ReportDetailDto>
    {
        public int Id { get; set; }

        public GetReport(int id)
        {
            Id = id;
        }
    }
}