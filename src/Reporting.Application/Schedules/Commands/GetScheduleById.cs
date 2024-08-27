using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetScheduleById : IRequest<ScheduleDto>
    {
        public int Id { get; set; }

        public GetScheduleById(int id)
        {
            Id = id;
        }
    }
}