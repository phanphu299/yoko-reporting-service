using System;
using Reporting.Domain.EnumType;

namespace Reporting.Application.Command.Model
{
    public class BaseScheduleContact
    {
        public Guid ObjectId { get; set; }
        public ContactType ObjectType { get; set; }
    }
}