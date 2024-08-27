using AHI.Infrastructure.Service.Tag.Model;

namespace Reporting.Domain.Entity
{
    public class EntityTagDb : EntityTag
    {
        public Template Template { get; set; }

        public Schedule Schedule { get; set; }
    }
}
