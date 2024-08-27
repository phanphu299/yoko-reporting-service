using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using Reporting.Application.Command.Model;
using Newtonsoft.Json;
using MediatR;

namespace Reporting.Application.Command
{
    public class UpdateStorage : IRequest<StorageDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TypeId { get; set; }
        public IDictionary<string, object> Content { get; set; }

        private static Func<UpdateStorage, Domain.Entity.Storage> Converter = Projection.Compile();

        public static Expression<Func<UpdateStorage, Domain.Entity.Storage>> Projection
        {
            get
            {
                return command => new Domain.Entity.Storage
                {
                    Id = command.Id,
                    Name = command.Name,
                    TypeId = command.TypeId,
                    Content = JsonConvert.SerializeObject(command.Content),
                    UpdatedUtc = DateTime.UtcNow
                };
            }
        }

        public static Domain.Entity.Storage Create(UpdateStorage command)
        {
            if (command == null)
                return null;
            return Converter(command);
        }
    }
}