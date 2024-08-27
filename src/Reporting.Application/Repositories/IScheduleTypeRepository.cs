using AHI.Infrastructure.Repository.Generic;

namespace Reporting.Application.Repository
{
    public interface IScheduleTypeRepository : IRepository<Domain.Entity.ScheduleType, string>
    {
    }
}