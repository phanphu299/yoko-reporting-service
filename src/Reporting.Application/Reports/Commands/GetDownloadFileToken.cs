using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetDownloadFileToken : IRequest<DownloadFileTokenDto>
    {
        public int Id { get; set; }
        public string StorageSpaceContent { get; set; }

        public GetDownloadFileToken(int id)
        {
            Id = id;
        }
    }
}