using System.Collections.Generic;

namespace Reporting.Application.Command
{
    public interface IUpsertSchedule
    {
        public string Period { get; set; }
        public IEnumerable<int> Templates { get; set; }
        public IEnumerable<int> Jobs { get; set; }
    }
}
