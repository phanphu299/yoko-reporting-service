namespace Reporting.Application.Command.Model
{
    public class DownloadFileUrlDto
    {
        public string DownloadFileUrl { get; set; }

        public DownloadFileUrlDto(string downloadFileUrl)
        {
            DownloadFileUrl = downloadFileUrl;
        }
    }
}