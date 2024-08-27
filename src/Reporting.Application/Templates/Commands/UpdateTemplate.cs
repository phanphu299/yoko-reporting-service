using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Reporting.Application.Constant;
using Reporting.Application.Command.Model;
using Newtonsoft.Json;
using MediatR;
using AHI.Infrastructure.Service.Tag.Model;

namespace Reporting.Application.Command
{
    public class UpdateTemplate : UpsertTagCommand, IRequest<TemplateDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public IDictionary<string, string> Default { get; private set; }
        public string OutputTypeId { get; set; }
        public int StorageId { get; set; }
        public IEnumerable<UpdateTemplateDetail> DataSets { get; set; }

        public UpdateTemplate()
        {
            DataSets = new List<UpdateTemplateDetail>();
            Default = new Dictionary<string, string>();
        }

        public void SetDefaultValue()
        {
            Default.Add(DefaultValue.REPORT_NAMTE, Name);
        }

        private static Func<UpdateTemplate, Domain.Entity.Template> Converter = Projection.Compile();

        public static Expression<Func<UpdateTemplate, Domain.Entity.Template>> Projection
        {
            get
            {
                return command => new Domain.Entity.Template
                    (command.DataSets.Select(x => UpdateTemplateDetail.Create(command.Id, x)).ToList())
                {
                    Id = command.Id,
                    Name = command.Name,
                    TemplateFileUrl = command.TemplateFileUrl,
                    Default = command.Default.Any() ? JsonConvert.SerializeObject(command.Default) : null,
                    StorageId = command.StorageId,
                    OutputTypeId = command.OutputTypeId,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.Template Create(UpdateTemplate command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }

    public class UpdateTemplateDetail : BaseTemplateDetail
    {
        private static Func<int, UpdateTemplateDetail, Domain.Entity.TemplateDetail> Converter = Projection.Compile();

        public static Expression<Func<int, UpdateTemplateDetail, Domain.Entity.TemplateDetail>> Projection
        {
            get
            {
                return (templateId, command) => new Domain.Entity.TemplateDetail
                {
                    Name = command.Name,
                    TemplateId = templateId,
                    DataSourceTypeId = command.DataSourceTypeId,
                    DataSourceContent = JsonConvert.SerializeObject(command.DataSourceContent),
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.TemplateDetail Create(int templateId, UpdateTemplateDetail command)
        {
            if (command == null)
                return null;
            return Converter(templateId, command);
        }
    }
}