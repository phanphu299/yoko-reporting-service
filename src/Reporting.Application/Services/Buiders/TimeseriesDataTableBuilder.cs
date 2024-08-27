using AHI.Infrastructure.SharedKernel.Extension;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Reporting.Application.Service
{
    public class TimeseriesDataTableBuilder : IAmDataTableBuilder
    {
        private string _assetId;
        private string _assetName;
        private string _timeZoneName;
        private string _dateTimeFormat;
        private RequestAmContent _requestContent;
        private byte[] _responseContent;
        private List<SnapshotSeriesResponse> _dataReponse;
        private List<TimeseriesFlatResponse> _dataResult;
        private DataSetAmMappingContent _dataSetContent;

        public TimeseriesDataTableBuilder(Guid assetId)
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
            _dataResult = new List<TimeseriesFlatResponse>();

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
            var countAggrAttrIds = _dataReponse.Where(x => !string.IsNullOrWhiteSpace(x.Timegrain) &&
                                                            x.RequestType == Default.SERIES &&
                                                            x.Aggregate == "count")
                                                .SelectMany(x => x.Attributes)
                                                .Select(x => x.AttributeId);
            var selectedAttributeIds = _requestContent.Attributes.Select(x => x.Id.ToString());
            var selectedAttributes = attributes.Where(x => selectedAttributeIds.Contains(x.AttributeId));
            _dataResult = selectedAttributes.SelectMany(attribute => attribute.Series
                                            .Select(seriesItem => new TimeseriesFlatResponse(seriesItem.Ts.ToLocalDateTime(_timeZoneName), attribute.AttributeNameNormalize, seriesItem.V.ExtractNumber(), CalculateDataType(countAggrAttrIds, attribute.AttributeId, attribute.DataType, seriesItem.V.ExtractNumber()), attribute.Uom?.Abbreviation)))
                                            .OrderByDescending(x => x.AHI_Timestamp)
                                            .ToList();

            return this;
        }

        private string CalculateDataType(IEnumerable<string> countAggAttrIds, string attributeId, string originalDataType, string value)
        {
            if (countAggAttrIds.Contains(attributeId))
                return "double";

            return (originalDataType == "bool" || originalDataType == "datetime") && !value.IsNumber() ? "text" : originalDataType;
        }

        public DataTable BuildDataTable()
        {
            var dataTable = ConvertAmDataToPivotDataTable(
                    source: _dataResult,
                    groupRows: new List<string> { nameof(TimeseriesFlatResponse.AHI_Timestamp) },
                    rowSelector: x => x.AHI_Timestamp,
                    columnSelector: x => x.AHI_AttributeNameNormalize,
                    dataSelector: x => x.AHI_Value
                );
            return dataTable;
        }

        private DataTable ConvertAmDataToPivotDataTable(
            IEnumerable<TimeseriesFlatResponse> source,
            IEnumerable<string> groupRows,
            Func<TimeseriesFlatResponse, DateTime> rowSelector,
            Func<TimeseriesFlatResponse, string> columnSelector,
            Func<TimeseriesFlatResponse, string> dataSelector)
        {
            var table = new DataTable();
            foreach (var rowName in groupRows)
            {
                table.Columns.Add(new DataColumn(rowName));
            }

            var columns = source.Select(columnSelector).Distinct();
            foreach (var column in columns)
            {
                var dataType = source.FirstOrDefault(x => x.AHI_AttributeNameNormalize == column)?.AHI_DataType;
                if (!string.Equals(dataType, "text", StringComparison.InvariantCultureIgnoreCase))
                {
                    table.Columns.Add(new DataColumn(column, typeof(decimal)));
                }
                else
                {
                    table.Columns.Add(new DataColumn(column, typeof(string)));
                }
                table.Columns.Add(new DataColumn($"{column}_Uom", typeof(string)));
            }

            var rows = source.GroupBy(rowSelector)
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = rowGroup.AsEnumerable()
                             });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();

                dataRow[0] = row.Key;
                foreach (var column in columns)
                {
                    var rowValue = row.Values.FirstOrDefault(x => x.AHI_AttributeNameNormalize == column);
                    if (rowValue != null)
                    {
                        if (!string.Equals(rowValue.AHI_DataType, "text", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (decimal.TryParse(rowValue.AHI_Value, out var num))
                            {
                                dataRow[column] = num;
                            }
                            else
                            {
                                dataRow[column] = DBNull.Value;
                            }
                        }
                        else
                        {
                            dataRow[column] = rowValue.AHI_Value;
                        }
                        dataRow[$"{column}_Uom"] = rowValue.AHI_Uom;
                    }
                    else
                    {
                        dataRow[column] = DBNull.Value;
                    }
                }

                table.Rows.Add(dataRow);
            }

            return table;
        }
    }
}