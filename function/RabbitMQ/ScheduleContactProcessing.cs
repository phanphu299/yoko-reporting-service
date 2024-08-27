using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.Azure.WebJobs;
using Reporting.Function.Constant;
using Reporting.Function.Model;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service.RabbitMQ
{
    public class ScheduleContactProcessing
    {
        private readonly ITenantContext _tenantContext;
        private readonly IContactService _contactService;

        public ScheduleContactProcessing(ITenantContext tenantContext, IContactService contactService)
        {
            _tenantContext = tenantContext;
            _contactService = contactService;
        }

        [FunctionName("ContactProcessing")]
        public Task ProcessContactChangedAsync(
            [RabbitMQTrigger("report.function.contact.changed.processing", ConnectionStringSetting = "RabbitMQ")] byte[] data)
        {
            BaseModel<ContactChangedMessage> request = data.Deserialize<BaseModel<ContactChangedMessage>>();
            var message = request.Message;
            _tenantContext.RetrieveFromString(message.TenantId, message.SubscriptionId);

            if (message.ActionType == AHI.Infrastructure.Bus.ServiceBus.Enum.ActionTypeEnum.Deleted)
            {
                switch (message.Status)
                {
                    case ContactStatus.REMOVED:
                        return _contactService.RemoveScheduleContactAsync(message.Id, ActionConstants.CONTACT_TYPE);
                    case ContactStatus.DELETED:
                        return _contactService.DeleteScheduleContactAsync(message.Id, ActionConstants.CONTACT_TYPE);
                }
            }
            else if (message.ActionType == AHI.Infrastructure.Bus.ServiceBus.Enum.ActionTypeEnum.Updated)
            {
                if (message.Status == ContactStatus.RESTORED)
                {
                    return _contactService.RestoreScheduleContactAsync(message.Id, ActionConstants.CONTACT_TYPE);
                }
            }
            return Task.CompletedTask;
        }

        [FunctionName("ContactGroupProcessing")]
        public Task ProcessContactGroupChangedAsync(
            [RabbitMQTrigger("report.function.contactgroup.changed.processing", ConnectionStringSetting = "RabbitMQ")] byte[] data)
        {
            BaseModel<ContactGroupChangedMessage> request = data.Deserialize<BaseModel<ContactGroupChangedMessage>>();
            var message = request.Message;
            _tenantContext.RetrieveFromString(message.TenantId, message.SubscriptionId);

            if (message.ActionType == AHI.Infrastructure.Bus.ServiceBus.Enum.ActionTypeEnum.Deleted)
            {
                return _contactService.RemoveScheduleContactAsync(message.Id, ActionConstants.CONTACT_GROUP_TYPE);
            }
            return Task.CompletedTask;
        }
    }
}