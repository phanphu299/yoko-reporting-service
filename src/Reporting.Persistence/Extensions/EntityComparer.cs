using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;
using Reporting.Domain.Entity;

namespace Reporting.Persistence.Extension
{
    public static class EntityComparer
    {
        public static readonly IEqualityComparer<SchedulerContact> ContactIdComparer = new IdEntityComparer<SchedulerContact>();
    }
    
    public class IdEntityComparer<T> : IEqualityComparer<T> where T : IEntity<Guid>
    {
        public bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}