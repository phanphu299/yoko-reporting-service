using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetStorage : IRequest<StorageDto>
    {
        public int Id { get; set; }

        public GetStorage(int id)
        {
            Id = id;
        }
    }
}