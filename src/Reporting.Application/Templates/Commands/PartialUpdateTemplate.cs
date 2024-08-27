using Microsoft.AspNetCore.JsonPatch;
using Reporting.Application.Command.Model;
using MediatR;
using AHI.Infrastructure.Service.Tag.Model;

namespace Reporting.Application.Command
{
    public class PartialUpdateTemplate : IRequest<TemplateDto>
    {
        public int Id { get; set; }
        public JsonPatchDocument JsonPatchDocument { set; get; }

        public PartialUpdateTemplate(int id, JsonPatchDocument jsonPatchDocument)
        {
            Id = id;
            JsonPatchDocument = jsonPatchDocument;
        }
    }
}