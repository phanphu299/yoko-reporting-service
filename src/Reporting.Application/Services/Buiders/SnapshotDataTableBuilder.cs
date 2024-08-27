using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;

namespace Reporting.Application.Service
{
    public class SnapshotDataTableBuilder : IAmDataTableBuilder
    {
        private string _assetId;
        private string _assetName;
        private string _timeZoneName;
        private string _dateTimeFormat;
        private RequestAmContent _requestContent;
        private byte[] _responseContent;
        private List<SnapshotSeriesResponse> _dataReponse;
        private List<SnapshotFlatResponse> _dataResult;
        private DataSetAmMappingContent _dataSetContent;

        public SnapshotDataTableBuilder(Guid assetId)
        {
            _assetId = assetId.ToString();
        }

        public void SetTimeZoneName(string timeZoneName)
        {
            _timeZoneName = timeZoneName;
        }

        public void SetDateTimeFormat(string dateTimeFormat)
        {
            _dateTimeFormat = dateTimeFormat;
        }

        public void SetResponseContent(byte[] responseContent)
        {
            _responseContent = responseContent;
        }

        public void SetRequestContent(RequestAmContent requestContent)
        {
            _requestContent = requestContent;
        }

        public void SetDataSetContent(DataSetAmMappingContent dataSetContent)
        {
            _dataSetContent = dataSetContent;
        }

        public IAmDataTableBuilder BuildDataArray()
        {
            _dataResult = new List<SnapshotFlatResponse>();

            if (_dataReponse == null)
            {
                var response = _responseContent.Deserialize<IEnumerable<SnapshotSeriesResponse>>();
                _dataReponse = response.ToList();
            }

            var firstResponseItem = _dataReponse.FirstOrDefault();
            if (firstResponseItem != null)
            {
                if (!string.IsNullOrEmpty(firstResponseItem.AssetName))
                    _assetName = firstResponseItem.AssetName;

                if (string.IsNullOrEmpty(_assetName))
                    _assetName = firstResponseItem.AssetId;
            }

            var attributes = _dataReponse.SelectMany(x => x.Attributes);
            var selectedAttributeIds = _requestContent.Attributes.Select(x => x.Id.ToString());
            foreach (var attribute in attributes)
            {
                if (selectedAttributeIds.Contains(attribute.AttributeId))
                {
                    var data = attribute.Series.Select(x => new SnapshotFlatResponse(GetLocalDateTime(x.Ts), _assetName, attribute.AttributeName, x.V, attribute.Uom?.Abbreviation));
                    if (!data.Any())
                    {
                        var item = new SnapshotFlatResponse(null, _assetName, attribute.AttributeName, string.Empty, string.Empty);
                        _dataResult.Add(item);
                    }
                    _dataResult.AddRange(data);
                }
            }

            return this;
        }

        public DataTable BuildDataTable()
        {
            return _dataResult.ToDataTable<SnapshotFlatResponse>();
        }

        private DateTime? GetLocalDateTime(long dateTimeUtcInMilliseconds)
        {
            if (dateTimeUtcInMilliseconds > 0)
                return dateTimeUtcInMilliseconds.ToLocalDateTime(_timeZoneName);
            return null;
        }
    }
}