using System.Threading.Tasks;
using Reporting.Function.Constant;

namespace Reporting.Function.Service.Abstraction
{
    public interface IErrorService
    {
        bool HasError { get; }
        Task RegisterErrorAsync(string messageCode, ErrorType errorType = ErrorType.UNDEFINED);
    }
}