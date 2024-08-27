using Microsoft.AspNetCore.JsonPatch;
using Reporting.Application.Command.Model;
using MediatR;

namespace Reporting.Application.Command
{
    public class PartialUpdateSchedule : IRequest<ScheduleDto>
    {
        public int Id { get; set; }
        public JsonPatchDocument JsonPatchDocument { set; get; }

        public PartialUpdateSchedule(int id, JsonPatchDocument jsonPatchDocument)
        {
            Id = id;
            JsonPatchDocument = jsonPatchDocument;
        }
    }
}