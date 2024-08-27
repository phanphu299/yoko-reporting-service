using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;
using Reporting.Persistence.Context;
using Reporting.Application.Repository;
using System.Collections.Generic;
using Reporting.Domain.Entity;

namespace Reporting.Persistence.Repository
{
    public class ScheduleContactRepository : GenericRepository<Domain.Entity.SchedulerContact, Guid>, IScheduleContactRepository
    {
        private readonly ReportingDbContext _context;

        public ScheduleContactRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<SchedulerContact> AsQueryable() => _context.ScheduleContacts;

        protected override void Update(SchedulerContact requestObject, SchedulerContact targetObject)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SchedulerContact>> GetScheduleContactsByScheduleIdAsync(int scheduleId)
        {
            return await AsQueryable().Where(x => x.ScheduleId == scheduleId).ToListAsync();
        }
    }
}