using Newtonsoft.Json;
using Reporting.Application.Constant;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AHI.Infrastructure.Security.Extension;

namespace Reporting.Application.Template.Command.Model
{
    public class TemplateDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TemplateId { get; set; }
        public string DataSourceTypeId { get; set; }
        public string DataSourceContent { get; set; }

        static Func<Domain.Entity.TemplateDetail, TemplateDetailDto> DtoConverter = DtoProjection.Compile();
        static Func<TemplateDetailDto, Domain.Entity.TemplateDetail> EntityConverter = EntityProjection.Compile();

        private static Expression<Func<Domain.Entity.TemplateDetail, TemplateDetailDto>> DtoProjection
        {
            get
            {
                return entity => new TemplateDetailDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TemplateId = entity.TemplateId,
                    DataSourceTypeId = entity.DataSourceTypeId,
                    DataSourceContent = GetDataSource(entity.DataSourceTypeId, entity.DataSourceContent, false),
                };
            }
        }

        public static TemplateDetailDto Create(Domain.Entity.TemplateDetail model)
        {
            if (model != null)
            {
                return DtoConverter(model);
            }
            return null;
        }

        private static Expression<Func<TemplateDetailDto, Domain.Entity.TemplateDetail>> EntityProjection
        {
            get
            {
                return dto => new Domain.Entity.TemplateDetail
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    TemplateId = dto.TemplateId,
                    DataSourceTypeId = dto.DataSourceTypeId,
                    DataSourceContent = GetDataSource(dto.DataSourceTypeId, dto.DataSourceContent, true),
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.TemplateDetail Create(TemplateDetailDto dto)
        {
            if (dto != null)
            {
                return EntityConverter(dto);
            }
            return null;
        }

        static string GetDataSource(string type, string dataSourceContent, bool decode)
        {
            if (type != DataSource.API_DATA_SOURCE)
                return dataSourceContent;

            var dataSource = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataSourceContent);
            if (dataSource.ContainsKey("endpoint"))
            {
                if (decode)
                    dataSource["endpoint"] = dataSource["endpoint"].ToString().Base64Decode();
                else
                    dataSource["endpoint"] = dataSource["endpoint"].ToString().Base64Encode();

                return JsonConvert.SerializeObject(dataSource);
            }
            return dataSourceContent;
        }
    }
}