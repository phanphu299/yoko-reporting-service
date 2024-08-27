using AHI.Infrastructure.Security.Extension;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Model;
using Reporting.Application.Template.Command.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class TemplateBasicDto : TagDtos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public string Default { get; set; }
        public int StorageId { get; set; }
        public string OutputTypeId { get; set; }
        public string ResourcePath { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public IEnumerable<TemplateDetailDto> DataSets { get; set; }
    }

    public class TemplateDto : TemplateBasicDto
    {
        public int DataSetCount { get; set; }
        public string CreatedBy { get; set; }
        public bool Deleted { get; set; }
        public OutputTypeDto OutputType { get; set; }
        public StorageDto Storage { get; set; }
        private static Func<Domain.Entity.Template, TemplateDto> Converter = Projection.Compile();
        static Func<Domain.Entity.Template, TemplateBasicDto> DtoConverter = DtoProjection.Compile();
        static Func<IEnumerable<TemplateBasicDto>, string, string, string, IEnumerable<Domain.Entity.Template>> EntityConverter = EntityProjection.Compile();

        public static Expression<Func<Domain.Entity.Template, TemplateDto>> Projection
        {
            get
            {
                return entity => new TemplateDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateFileUrl = entity.TemplateFileUrl,
                    Default = entity.Default,
                    StorageId = entity.StorageId,
                    OutputTypeId = entity.OutputTypeId,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    CreatedBy = entity.CreatedBy,
                    ResourcePath = entity.ResourcePath,
                    OutputType = OutputTypeDto.Create(entity.OutputType),
                    Storage = StorageDto.Create(entity.Storage),
                    Deleted = entity.Deleted,
                    Tags = entity.EntityTags.MappingTagDto()
                };
            }
        }

        private static Expression<Func<Domain.Entity.Template, TemplateBasicDto>> DtoProjection
        {
            get
            {
                return entity => new TemplateBasicDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    FilePath = !string.IsNullOrWhiteSpace(entity.TemplateFileUrl) ? entity.TemplateFileUrl.Base64Encode() : null,
                    Default = entity.Default,
                    StorageId = entity.StorageId,
                    OutputTypeId = entity.OutputTypeId,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    ResourcePath = entity.ResourcePath,
                    DataSets = entity.Details.Select(TemplateDetailDto.Create).ToList(),
                    FileName = GetFileName(entity.TemplateFileUrl)
                };
            }
        }

        private static Expression<Func<IEnumerable<TemplateBasicDto>, string, string, string, IEnumerable<Domain.Entity.Template>>> EntityProjection
        {
            get
            {
                return (templates, upn, subscriptionId, projectId) =>
                templates.Select(template => new Domain.Entity.Template()
                {
                    Id = template.Id,
                    Name = template.Name,
                    TemplateFileUrl = GetTemplateUrl(template.FilePath),
                    Default = template.Default,
                    StorageId = template.StorageId,
                    OutputTypeId = template.OutputTypeId,
                    ResourcePath = template.ResourcePath,
                    CreatedBy = upn,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                });
            }
        }

        public static IEnumerable<Domain.Entity.Template> Create(IEnumerable<TemplateDto> models, string upn, string subscriptionId, string projectId)
        {
            if (models.Any())
            {
                return EntityConverter(models, upn, subscriptionId, projectId);
            }
            return new List<Domain.Entity.Template>();
        }

        public static TemplateDto Create(Domain.Entity.Template entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }

        public static TemplateBasicDto CreateArchiveDto(Domain.Entity.Template model)
        {
            if (model != null)
            {
                return DtoConverter(model);
            }
            return null;
        }

        private static string GetTemplateUrl(string templateUrl)
        {
            return templateUrl.Base64Decode();
        }

        private static string GetFileName(string templateUrl)
        {
            if (string.IsNullOrEmpty(templateUrl))
                return string.Empty;
            var templateFileUrlPath = templateUrl.Substring(templateUrl.IndexOf("report_template"));
            var arrayPath = templateFileUrlPath.Split('/');
            return $"{arrayPath[2]}";
        }
    }
}