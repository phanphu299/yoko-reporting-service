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
    public class AddTemplate : UpsertTagCommand, IRequest<TemplateDto>
    {
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public IDictionary<string, string> Default { get; private set; }
        public string OutputTypeId { get; set; }
        public int StorageId { get; set; }
        public IEnumerable<AddTemplateDetail> DataSets { get; set; }

        public AddTemplate()
        {
            DataSets = new List<AddTemplateDetail>();
            Default = new Dictionary<string, string>();
        }

        public void SetDefaultValue()
        {
            Default.Add(DefaultValue.REPORT_NAMTE, Name);
        }

        private static Func<AddTemplate, Domain.Entity.Template> Converter = Projection.Compile();

        public static Expression<Func<AddTemplate, Domain.Entity.Template>> Projection
        {
            get
            {
                return command => new Domain.Entity.Template
                    (command.DataSets.Select(x => AddTemplateDetail.Create(x)).ToList())
                {
                    Name = command.Name,
                    TemplateFileUrl = command.TemplateFileUrl,
                    Default = JsonConvert.SerializeObject(command.Default),
                    StorageId = command.StorageId,
                    OutputTypeId = command.OutputTypeId,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.Template Create(AddTemplate command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }

    public class BaseTemplateDetail
    {
        public string Name { get; set; }
        public string DataSourceTypeId { get; set; }
        public IDictionary<string, object> DataSourceContent { get; set; }
    }

    public class AddTemplateDetail : BaseTemplateDetail
    {
        private static Func<AddTemplateDetail, Domain.Entity.TemplateDetail> Converter = Projection.Compile();

        public static Expression<Func<AddTemplateDetail, Domain.Entity.TemplateDetail>> Projection
        {
            get
            {
                return command => new Domain.Entity.TemplateDetail
                {
                    Name = command.Name,
                    DataSourceTypeId = command.DataSourceTypeId,
                    DataSourceContent = JsonConvert.SerializeObject(command.DataSourceContent),
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.TemplateDetail Create(AddTemplateDetail command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }
}