using System.IO;
using System.Linq;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Reporting.Application.Extension;

namespace Reporting.Application.Service
{
    public class DataTableBuilder
    {
        private Stream _responseContent;
        private JObject _jObjectResponse;
        private JArray _jArrayReponse;
        private JArray _jArrayResult;
        private DataSetMappingContent _dataSetContent;

        public DataTableBuilder()
        {
        }

        public void SetResponseContent(Stream responseContent)
        {
            _responseContent = responseContent;
        }

        public void SetDataSetContent(DataSetMappingContent dataSetContent)
        {
            _dataSetContent = dataSetContent;
        }

        public DataTableBuilder BuildJsonArray()
        {
            _jArrayReponse = _responseContent.TryExtractJArray();
            if (_jArrayReponse == null)
            {
                _jObjectResponse = _responseContent.TryExtractJObject();
                _jArrayReponse = _jObjectResponse[_dataSetContent.PropName] as JArray;
            }
            return this;
        }

        /// <summary>
        /// do mapping here if we want to map source object to different object
        /// </summary>
        /// <returns></returns>
        public DataTableBuilder BuidMappingField()
        {
            _jArrayResult = new JArray();
            foreach (JObject jObjectResponse in _jArrayReponse)
            {
                if (jObjectResponse.Properties().Any())
                {
                    _jArrayResult.Add(jObjectResponse);
                }
            }
            return this;
        }

        public DataTable BuildDataTable()
        {
            return JsonConvert.DeserializeObject<DataTable>(_jArrayResult.ToString());
        }
    }
}