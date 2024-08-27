namespace Reporting.Application.Command.Model
{
    public class PreviewReportDto
    {
        public string FileUrl { get; set; }

        public PreviewReportDto(string fileUrl)
        {
            FileUrl = fileUrl;
        }
    }
}