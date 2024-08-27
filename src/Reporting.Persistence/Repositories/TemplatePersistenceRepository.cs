using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;
using Reporting.Application.Constant;
using Reporting.Application.Repository;
using Reporting.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reporting.Persistence.Repository
{
    public class TemplatePersistenceRepository : GenericRepository<Domain.Entity.Template, int>, ITemplateRepository
    {
        private readonly ReportingDbContext _context;

        public TemplatePersistenceRepository(ReportingDbContext context) : base(context)
        {
            _context = context;
        }

        public override IQueryable<Domain.Entity.Template> AsQueryable()
        {
            return _context.Templates
               .Include(x => x.OutputType)
               .Include(x => x.EntityTags)
               .Where(x => !x.Deleted && (!x.EntityTags.Any() || x.EntityTags.Any(a => a.EntityType == EntityTypeConstants.REPORT_TEMPLATE)));
        }

        public override IQueryable<Domain.Entity.Template> AsFetchable()
        {
            return _context.Templates.AsNoTracking().AsQueryable().Select(x => new Domain.Entity.Template { Id = x.Id, Name = x.Name });
        }

        protected override void Update(Domain.Entity.Template requestObject, Domain.Entity.Template targetObject)
        {
            throw new NotImplementedException();
        }

        public override Task<Domain.Entity.Template> FindAsync(int id)
        {
            return _context.Templates.Include(x => x.OutputType)
                                        .Include(x => x.Storage)
                                        .Include(x => x.Details)
                                        .ThenInclude(x => x.DataSourceType)
                                        .Include(x => x.EntityTags)
                                        .Where(x => !x.Deleted && x.Id == id && (!x.EntityTags.Any() || x.EntityTags.Any(a => a.EntityType == EntityTypeConstants.REPORT_TEMPLATE)))
                                        .FirstOrDefaultAsync();
        }

        public async Task<Domain.Entity.Template> PartialUpdateTemplateAsync(Domain.Entity.Template requestObject, Domain.Entity.Template targetObject)
        {
            if (!string.IsNullOrEmpty(requestObject.Name))
                targetObject.Name = requestObject.Name;

            if (!string.IsNullOrEmpty(requestObject.TemplateFileUrl))
                targetObject.TemplateFileUrl = requestObject.TemplateFileUrl;

            if (!string.IsNullOrEmpty(requestObject.Default))
                targetObject.Default = requestObject.Default;

            if (requestObject.StorageId != 0)
                targetObject.StorageId = requestObject.StorageId;

            if (!string.IsNullOrEmpty(requestObject.OutputTypeId))
                targetObject.OutputTypeId = requestObject.OutputTypeId;

            if (requestObject.Details.Any())
            {
                _context.RemoveRange(targetObject.Details);
                _context.AddRange(requestObject.Details);
            }

            targetObject.UpdatedUtc = requestObject.UpdatedUtc;
            await _context.SaveChangesAsync();
            return targetObject;
        }

        public async Task RemoveTemplatesAsync(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var e = await _context.Templates.FirstOrDefaultAsync(x => x.Id == id);
                if (e == null)
                {
                    throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
                }
                e.Deleted = true;
                e.UpdatedUtc = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task RetrieveTemplateAsync(IEnumerable<Domain.Entity.Template> templates)
        {
            _context.Database.SetCommandTimeout(RetrieveConstants.TIME_OUT);
            // enable insert identity and drop constraint
            var enableSql = @"SET IDENTITY_INSERT templates ON;
                              IF EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'templates') and name = 'fk_templates_storage_id')
                              BEGIN 
                                 ALTER TABLE templates DROP CONSTRAINT fk_templates_storage_id;
                              END";

            await _context.Database.ExecuteSqlRawAsync(enableSql);

            await _context.AddRangeAsync(templates);
            // Need save change before disable insert identity
            await _context.SaveChangesAsync();

            // disable insert identity and add constraint
            var disableSql = @"SET IDENTITY_INSERT templates OFF;
                               IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID(N'templates') and name = 'fk_templates_storage_id')
                               BEGIN
                                  ALTER TABLE templates ADD CONSTRAINT fk_templates_storage_id FOREIGN key(storage_id) REFERENCES storages(id);
                               END";


            await _context.Database.ExecuteSqlRawAsync(disableSql);
        }

        public async Task RetrieveTemplateDetailAsync(IEnumerable<Domain.Entity.TemplateDetail> details)
        {
            _context.Database.SetCommandTimeout(RetrieveConstants.TIME_OUT);
            // enable insert identity
            var enableSql = @"SET IDENTITY_INSERT template_details ON;";
            await _context.Database.ExecuteSqlRawAsync(enableSql);

            await _context.AddRangeAsync(details);
            // Need save change before disable insert identity
            await _context.SaveChangesAsync();

            // disable insert identity
            var disableSql = @"SET IDENTITY_INSERT template_details OFF;";
            await _context.Database.ExecuteSqlRawAsync(disableSql);
        }
    }
}