using System.Collections.Generic;
using Reporting.Function.Model;

namespace Reporting.Function.Service.Abstraction
{
    public interface IExportTrackingService : IErrorService
    {
        ICollection<TrackError> GetErrors { get; }
    }
}