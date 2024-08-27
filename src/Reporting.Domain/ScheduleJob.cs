using AHI.Infrastructure.Repository.Model.Generic;

namespace Reporting.Domain.Entity
{
    public class ScheduleJob : IEntity<int>
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int ScheduleId { get; set; }
        public Schedule Job { get; set; }
        public Schedule Schedule { get; set; }
    }
}