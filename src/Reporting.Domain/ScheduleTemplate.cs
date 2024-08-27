namespace Reporting.Domain.Entity
{
    public class ScheduleTemplate
    {
        public int TemplateId { get; set; }
        public int ScheduleId { get; set; }
        public Template Template { get; set; }
        public Schedule Schedule { get; set; }
    }
}