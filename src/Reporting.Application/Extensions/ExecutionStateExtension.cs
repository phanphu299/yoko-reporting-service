using Reporting.Domain.EnumType;

namespace Reporting.Application.Extension
{
    public static class ExecutionStateExtension
    {
        public static string GetString(this ExecutionState state)
        {
            return state switch
            {
                ExecutionState.INIT => "initialized",
                ExecutionState.RUN => "running",
                ExecutionState.FIN => "completed",
                ExecutionState.PFIN => "completed partially",
                ExecutionState.FAIL => "failed",
                _ => string.Empty
            };
        }
    }
}