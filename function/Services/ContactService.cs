using System;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class ContactService : IContactService
    {
        private readonly ITenantContext _tenantContext;
        private readonly IMasterService _masterService;
        private readonly IContactRepository _contactRepository;

        public ContactService(ITenantContext tenantContext, IMasterService masterService, IContactRepository contactRepository)
        {
            _tenantContext = tenantContext;
            _masterService = masterService;
            _contactRepository = contactRepository;
        }

        public async Task DeleteScheduleContactAsync(Guid objectId, string objectType)
        {
            var projects = await _masterService.GetAllProjectBySubscriptionAsync();
            foreach (var project in projects)
            {
                _tenantContext.SetProjectId(project.Id);
                await _contactRepository.UpdateContactByProjectAsync(objectId, objectType, deleted: true);
            }
        }

        public async Task RestoreScheduleContactAsync(Guid objectId, string objectType)
        {
            var projects = await _masterService.GetAllProjectBySubscriptionAsync();
            foreach (var project in projects)
            {
                _tenantContext.SetProjectId(project.Id);
                await _contactRepository.UpdateContactByProjectAsync(objectId, objectType, deleted: false);
            }
        }

        public async Task RemoveScheduleContactAsync(Guid objectId, string objectType)
        {
            var projects = await _masterService.GetAllProjectBySubscriptionAsync();
            foreach (var project in projects)
            {
                _tenantContext.SetProjectId(project.Id);
                await _contactRepository.RemoveContactByProjectAsync(objectId, objectType);
            }
        }
    }
}