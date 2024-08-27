using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Newtonsoft.Json;
using Npgsql;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class PostgreSQLDataSourceHandler : IDataSourceHandler
    {
        private readonly ITenantContext _tenantContext;
        private readonly IRequestDataManager _requestDataManager;

        public PostgreSQLDataSourceHandler(ITenantContext tenantContext, IRequestDataManager requestDataManager)
        {
            _tenantContext = tenantContext;
            _requestDataManager = requestDataManager;
        }

        public Task<Dictionary<string, string>> CheckExistDataSourceAsync(CheckExistDataSource command)
        {
            // TODO: Not implement yet
            return Task.FromResult(new Dictionary<string, string>());
        }

        public async Task<IEnumerable<DataTableResult>> HandleAsync(GetDataSource command)
        {
            try
            {
                var result = new List<DataTableResult>();
                var dbContent = JsonConvert.DeserializeObject<DbContent>(command.CurrentDataSourceContent);
                using (var dbConnection = new NpgsqlConnection(dbContent.ConnectionString))
                {
                    await dbConnection.OpenAsync();
                    foreach (var dataSet in dbContent.DataSets)
                    {
                        var query = _requestDataManager.EnrichSqlQuery(dataSet.Query, command.FromDateUtcInMilliseconds, command.ToDateUtcInMilliseconds);
                        var cmd = new NpgsqlCommand(query, dbConnection);
                        var da = new NpgsqlDataAdapter(cmd);
                        var dt = new DataTable();
                        var ds = new DataSet();
                        da.Fill(ds);
                        dt = ds.Tables[0];
                        result.Add(new DataTableResult(dataSet.Name, dt));
                    }
                    await dbConnection.CloseAsync();
                }
                return result;
            }
            catch (PostgresException ex)
            {
                throw new GenericProcessFailedException(detailCode: Message.POSTGRE_QUERY_FAILED, innerException: ex);
            }
        }
    }
}