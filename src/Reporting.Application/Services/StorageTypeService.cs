using System;
using AHI.Infrastructure.Service;
using Reporting.Application.Repository;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class StorageTypeService : BaseSearchService<Domain.Entity.StorageType, string, SearchStorageType, StorageTypeDto>, IStorageTypeService
    {
        public StorageTypeService(IServiceProvider serviceProvider)
             : base(StorageTypeDto.Create, serviceProvider)
        {
        }

        protected override Type GetDbType()
        {
            return typeof(IStorageTypeRepository);
        }
    }
}