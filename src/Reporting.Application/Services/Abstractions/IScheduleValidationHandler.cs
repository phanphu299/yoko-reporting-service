using Reporting.Application.Command;
using System.Threading.Tasks;

namespace Reporting.Application.Service.Abstraction
{
    public interface IScheduleValidationHandler
    {
        Task HandleAsync<T>(T command) where T : IUpsertSchedule;
    }
}