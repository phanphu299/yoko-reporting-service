using System;
using System.Threading.Tasks;

namespace Reporting.Function.Service.Abstraction
{
    public interface IContactService
    {
        /// <summary>
        ///     Hard delete schedule contact, when the specified contact was hard deleted alongside its related user.
        /// </summary>
        /// <param name="objectId">Id of the contact or contact group</param>
        /// <param name="objectType">Type of the schedule contact</param>
        /// <returns></returns>
        Task RemoveScheduleContactAsync(Guid objectId, string objectType);

        /// <summary>
        ///     Soft delete schedule contacts, when the specified contact was soft deleted alongside its related user.
        /// </summary>
        /// <param name="objectId">Id of the contact or contact group</param>
        /// <param name="objectType">Type of the schedule contact</param>
        /// <returns></returns>
        Task DeleteScheduleContactAsync(Guid objectId, string objectType);

        /// <summary>
        ///     Restore soft deleted schedule contacts, when the specified contact was restored alongside its related user.
        /// </summary>
        /// <param name="objectId">Id of the contact or contact group</param>
        /// <param name="objectType">Type of the schedule contact</param>
        /// <returns></returns>
        Task RestoreScheduleContactAsync(Guid objectId, string objectType);
    }
}