using System.Collections.Generic;

namespace Reporting.Application.Command.Model
{
    public class SimpleTemplateByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileUrl { get; set; }
        public IDictionary<string, object> Default { get; set; }
        public OutputTypeDto OutputType { get; set; }
        public StorageDto Storage { get; set; }
    }
}