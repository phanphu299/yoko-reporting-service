using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class SqlServerDataSourceHandler : IDataSourceHandler
    {
        private readonly ITenantContext _tenantContext;
        private readonly IRequestDataManager _requestDataManager;

        public SqlServerDataSourceHandler(ITenantContext tenantContext, IRequestDataManager requestDataManager)
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
                using (var dbConnection = new SqlConnection(dbContent.ConnectionString))
                {
                    await dbConnection.OpenAsync();
                    foreach (var dataSet in dbContent.DataSets)
                    {
                        var dt = new DataTable();
                        var query = _requestDataManager.EnrichSqlQuery(dataSet.Query, command.FromDateUtcInMilliseconds, command.ToDateUtcInMilliseconds);
                        var dataAdapter = new SqlDataAdapter(query, dbConnection);
                        dataAdapter.Fill(dt);
                        result.Add(new DataTableResult(dataSet.Name, dt));
                    }
                    await dbConnection.CloseAsync();
                }
                return result;
            }
            catch (SqlException ex)
            {
                throw new GenericProcessFailedException(detailCode: Message.SQL_SERVER_QUERY_FAILED, innerException: ex);
            }
        }
    }

    public class DbContent
    {
        public string ConnectionString { get; set; }
        public IEnumerable<DbDataSetContent> DataSets { get; set; }

        public DbContent()
        {
            DataSets = new List<DbDataSetContent>();
        }
    }

    public class DbDataSetContent
    {
        public string Name { get; set; }
        public string Query { get; set; }
    }
}