using System;
using System.Linq.Expressions;

namespace Reporting.Application.Command.Model
{
    public class StorageTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool CanRead { get; set; }

        private static Func<Domain.Entity.StorageType, StorageTypeDto> Converter = Projection.Compile();

        public static Expression<Func<Domain.Entity.StorageType, StorageTypeDto>> Projection
        {
            get
            {
                return entity => new StorageTypeDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    CanRead = entity.CanRead
                };
            }
        }

        public static StorageTypeDto Create(Domain.Entity.StorageType entity)
        {
            if (entity == null)
                return null;
            return Converter(entity);
        }
    }
}