using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Reporting.Application.Command.Model;
using Newtonsoft.Json;
using MediatR;

namespace Reporting.Application.Command
{
    public class AddStorage : IRequest<StorageDto>
    {
        public string Name { get; set; }
        public string TypeId { get; set; }
        public IDictionary<string, object> Content { get; set; }

        private static Func<AddStorage, Domain.Entity.Storage> Converter = Projection.Compile();

        public static Expression<Func<AddStorage, Domain.Entity.Storage>> Projection
        {
            get
            {
                return command => new Domain.Entity.Storage
                {
                    Name = command.Name,
                    TypeId = command.TypeId,
                    Content = JsonConvert.SerializeObject(command.Content),
                    CanEdit = true,
                    CanDelete = true,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.Storage Create(AddStorage command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }
}