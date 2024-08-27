using MediatR;
using Reporting.Application.Command.Model;

namespace Reporting.Application.Command
{
    public class GetDownloadFileUrl : IRequest<DownloadFileUrlDto>
    {
        public int Id { get; set; }
        public string StorageUrl { get; set; }
        public string StorageSpaceContent { get; set; }

        public GetDownloadFileUrl(int id)
        {
            Id = id;
        }
    }
}