namespace Reporting.Function.Model
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? TemplateId { get; set; }
        public string TemplateName { get; set; }
        public int? ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public string FileName { get; set; }
        public string StorageUrl { get; set; }
        public string StorageContent { get; set; }
        public string StorageType { get; set; }
    }
}