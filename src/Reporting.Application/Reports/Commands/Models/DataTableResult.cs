using System.Data;

namespace Reporting.Application.Command.Model
{
    public class DataTableResult
    {
        public string Name { get; set; }
        public DataTable Table { get; set; }

        public DataTableResult(string name, DataTable table)
        {
            Name = name;
            Table = table;
        }
    }
}