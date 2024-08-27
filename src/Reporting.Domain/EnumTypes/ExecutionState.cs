using System.ComponentModel;

namespace Reporting.Domain.EnumType
{
    public enum ExecutionState
    {
        [Description("Initial")]
        INIT,

        [Description("Running")]
        RUN,
        
        [Description("Finished/Completed")]
        FIN,

        [Description("Partial finished/completed")]
        PFIN,

        [Description("Failed")]
        FAIL
    }
}