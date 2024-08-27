using System;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class ContactRepository : IContactRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;

        public ContactRepository(ITenantContext tenantContext, IConfiguration configuration)
        {
            _tenantContext = tenantContext;
            _configuration = configuration;
        }

        public async Task UpdateContactByProjectAsync(Guid objectId, string objectType, bool deleted)
        {
            var query = @$"UPDATE schedule_contacts SET deleted = @Deleted WHERE object_id = @ObjectId AND object_type = @ObjectType";
            var connectionString = _configuration["ConnectionStrings:Default"].BuildConnectionString(_configuration, _tenantContext.ProjectId);
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                ObjectId = objectId,
                                ObjectType = objectType,
                                Deleted = deleted
                            },
                            transaction, commandTimeout: 600);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public async Task RemoveContactByProjectAsync(Guid objectId, string objectType)
        {
            var query = "DELETE FROM schedule_contacts WHERE object_id = @ObjectId AND object_type = @ObjectType";
            var connectionString = _configuration["ConnectionStrings:Default"].BuildConnectionString(_configuration, _tenantContext.ProjectId);
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                ObjectId = objectId,
                                ObjectType = objectType
                            },
                            transaction, commandTimeout: 600);
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }
    }
}
