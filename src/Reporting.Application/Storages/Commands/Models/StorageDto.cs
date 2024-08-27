using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class StorageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public string Content { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public StorageTypeDto Type { get; set; }

        private static Func<Domain.Entity.Storage, StorageDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.Storage, StorageDto>> Projection
        {
            get
            {
                return entity => new StorageDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    TypeId = entity.TypeId,
                    Content = entity.Content,
                    CanEdit = entity.CanEdit,
                    CanDelete = entity.CanDelete,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    Type = StorageTypeDto.Create(entity.Type)
                };
            }
        }

        public static StorageDto Create(Domain.Entity.Storage entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}