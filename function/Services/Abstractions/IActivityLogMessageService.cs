using System.Threading.Tasks;

namespace Reporting.Function.Service.Abstraction
{
    public interface IActivityLogMessageService
    {
        Task<string> GetMessageAsync(string messageCode);
    }
}