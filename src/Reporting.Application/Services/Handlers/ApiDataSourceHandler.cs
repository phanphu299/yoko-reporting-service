using AHI.Infrastructure.Exception;
using Newtonsoft.Json;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Service.Abstraction;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Application.Service
{
    public class ApiDataSourceHandler : IDataSourceHandler
    {
        private static HttpClient _httpClient;
        private readonly IRequestDataManager _requestDataManager;

        public ApiDataSourceHandler(IRequestDataManager requestDataManager)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }
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
                var requestContent = JsonConvert.DeserializeObject<RequestContent>(command.CurrentDataSourceContent);
                InitRequest(requestContent);
                HttpResponseMessage response = null;
                string url = _requestDataManager.EnrichRequestUrl(requestContent.Endpoint, command.FromDateUtcInMilliseconds, command.ToDateUtcInMilliseconds);
                switch (requestContent.Method.ToLower())
                {
                    case HttpRequestMethod.GET:
                        response = await _httpClient.GetAsync(url);
                        break;
                    case HttpRequestMethod.POST:
                        response = await _httpClient.PostAsync(
                            url,
                            new StringContent(JsonConvert.SerializeObject(requestContent.Body), Encoding.UTF8, MediaType.APPLICATION_JSON)
                        );
                        break;
                    default:
                        break;
                }

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStreamAsync();
                var dataTableBuilder = new DataTableBuilder();
                var result = new List<DataTableResult>();

                dataTableBuilder.SetResponseContent(responseContent);
                foreach (var dataSetContent in requestContent.DataSetMappings)
                {
                    dataTableBuilder.SetDataSetContent(dataSetContent);
                    var dataTableResult = dataTableBuilder.BuildJsonArray().BuidMappingField().BuildDataTable();
                    result.Add(new DataTableResult(dataSetContent.DataSetName, dataTableResult));
                }

                return result;
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.API_CALL_FAILED);
            }
        }

        private void InitRequest(RequestContent requestContent)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            requestContent.Endpoint = $"{requestContent.Endpoint}?{requestContent.Query}";
            foreach (var header in requestContent.Headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
            }
        }
    }

    public class RequestContent
    {
        public string Endpoint { get; set; }
        public string Method { get; set; }
        public string Query { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public IDictionary<string, object> Body { get; set; }
        public IEnumerable<DataSetMappingContent> DataSetMappings { get; set; }

        public RequestContent()
        {
            Headers = new Dictionary<string, object>();
            Body = new Dictionary<string, object>();
            DataSetMappings = new List<DataSetMappingContent>();
        }
    }

    public class DataSetMappingContent
    {
        public string PropName { get; set; }
        public string DataSetName { get; set; }
    }
}