using System;

namespace Reporting.Function.Model
{
    public class ContactChangedMessage : BaseMessage
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }
}
