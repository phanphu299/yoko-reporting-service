namespace Reporting.Application.Command.Model
{
    public class DownloadFileTokenDto
    {
        public string DownloadFileToken { get; set; }

        public DownloadFileTokenDto(string downloadFileToken)
        {
            DownloadFileToken = downloadFileToken;
        }
    }
}