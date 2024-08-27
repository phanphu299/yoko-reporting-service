using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.Service.Tag.Extension;

namespace Reporting.Application.Command.Model
{
    public class TemplateByIdDto : TagDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public IDictionary<string, object> Default { get; set; }
        public int StorageId { get; set; }
        public string OutputTypeId { get; set; }
        public string CreatedBy { get; set; }
        public string ResourcePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public OutputTypeDto OutputType { get; set; }
        public StorageDto Storage { get; set; }

        public IEnumerable<DataSourceContentDto> DataSets { get; set; }

        private static Func<Domain.Entity.Template, TemplateByIdDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Template, TemplateByIdDto>> Projection
        {
            get
            {
                return entity => new TemplateByIdDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateFileUrl = entity.TemplateFileUrl,
                    Default = JsonConvert.DeserializeObject<IDictionary<string, object>>(entity.Default),
                    StorageId = entity.StorageId,
                    OutputTypeId = entity.OutputTypeId,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    OutputType = OutputTypeDto.Create(entity.OutputType),
                    Storage = StorageDto.Create(entity.Storage),
                    DataSets = entity.Details.Select(DataSourceContentDto.Create),
                    Tags = entity.EntityTags.MappingTagDto()
                };
            }
        }

        public static TemplateByIdDto Create(Domain.Entity.Template entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }

        public SimpleTemplateByIdDto CreateSimpleTemplate()
        {
            return new SimpleTemplateByIdDto() { Id = Id, Name = Name, TemplateFileUrl = TemplateFileUrl, Default = Default, OutputType = OutputType, Storage = Storage };
        }
    }
}