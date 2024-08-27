using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.UserContext.Abstraction;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pipelines.Sockets.Unofficial.Arenas;
using Reporting.Application.Command;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Enum;
using Reporting.Application.Extension;
using Reporting.Application.Service.Abstraction;
using Reporting.Application.Services.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Reporting.Application.Service
{
    public class AmDataSourceHandler : IDataSourceHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestDataManager _requestDataManager;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly ILogger<AmDataSourceHandler> _logger;

        public AmDataSourceHandler(
            IHttpClientFactory httpClientFactory,
            IRequestDataManager requestDataManager,
            ITenantContext tenantContext,
            IUserContext userContext,
            ILogger<AmDataSourceHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _requestDataManager = requestDataManager;
            _tenantContext = tenantContext;
            _userContext = userContext;
            _logger = logger;
        }

        public async Task<Dictionary<string, string>> CheckExistDataSourceAsync(CheckExistDataSource command)
        {
            var requestContent = JsonConvert.DeserializeObject<RequestAmContent>(command.CurrentDataSourceContent);
            var tenantContext = _tenantContext.Clone();
            tenantContext.RetrieveFromString(requestContent.TenantId, requestContent.SubscriptionId, requestContent.ProjectId);

            try
            {
                AMDataSourceValidator subscriptionValidator = new SubscriptionValidator(tenantContext, _httpClientFactory);
                AMDataSourceValidator projectValidator = new ProjectValidator(tenantContext, _httpClientFactory);
                AMDataSourceValidator assetValidator = new AssetValidator(tenantContext, _httpClientFactory);
                AMDataSourceValidator attributeValidator = new AttributeValidator(tenantContext, _httpClientFactory);
                subscriptionValidator.SetNextValidator(projectValidator);
                projectValidator.SetNextValidator(assetValidator);
                assetValidator.SetNextValidator(attributeValidator);

                return await subscriptionValidator.ValidateAsync(requestContent);
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.AM_CALL_FAILED);
            }
        }

        public async Task<IEnumerable<DataTableResult>> HandleAsync(GetDataSource command)
        {
            try
            {
                var requestContent = JsonConvert.DeserializeObject<RequestAmContent>(command.CurrentDataSourceContent);
                var dataSetContent = requestContent.DataSetMappings.First();
                var tenantContext = _tenantContext.Clone();
                tenantContext.RetrieveFromString(requestContent.TenantId, requestContent.SubscriptionId, requestContent.ProjectId);
                var httpClient = _httpClientFactory.CreateClient(HttpClientName.DEVICE_SERVICE, tenantContext);
                var result = new List<DataTableResult>();
                byte[] responseContent = null;
                HttpResponseMessage response = null;
                IAmDataTableBuilder builder = null;
                List<SeriesRequestItem> requestItemList = new List<SeriesRequestItem>();
                string url = "dev/assets/series?timeout=30";

                if (requestContent.DataType == DataType.SNAPSHOT)
                {
                    builder = new SnapshotDataTableBuilder(requestContent.AssetId);
                    foreach (var attribute in requestContent.Attributes)
                    {
                        var requestItem = new SeriesRequestItem(requestContent.SubscriptionId,
                                                            requestContent.ProjectId,
                                                            Default.SNAPSHOT,
                                                            requestContent.AssetId,
                                                            new string[] { attribute.Id.ToString() },
                                                            command.FromDateUtcInMilliseconds,
                                                            command.ToDateUtcInMilliseconds,
                                                            Default.TIMEGRAIN,
                                                            Default.AGGREGATION,
                                                            useCustomTimeRange: true);
                        requestItemList.Add(requestItem);
                    }

                    var payloadItem = new SeriesRequest(requestItemList);
                    payloadItem.TimezoneOffSet = DateTimeExtension.GetTimeZoneOffset(command.TimeZoneName);
                    var payloadJson = JsonConvert.SerializeObject(payloadItem);
                    response = await httpClient.PostAsync(
                        url,
                        new StringContent(payloadJson, Encoding.UTF8, MediaType.APPLICATION_JSON)
                    );
                }
                else if (requestContent.DataType == DataType.TIMESERIES)
                {
                    builder = new TimeseriesDataTableBuilder(requestContent.AssetId);
                    // filter attributes
                    var datasetColumnNames = GetDataSetColumnNames(command.TemplateFile, dataSetContent.DataSetName);
                    var datasetAttributeIds = Array.Empty<string>();
                    if (datasetColumnNames.Any())
                    {
                        var detailResponse = await httpClient.GetAsync($"dev/assets/{requestContent.AssetId}");
                        var detailResponseContent = await detailResponse.ReadContentAsync<AssetDto>();
                        datasetAttributeIds = detailResponseContent.Attributes.Where(x => datasetColumnNames.Contains(x.NormalizeName.ToLower())).Select(x => x.Id).ToArray();
                    }

                    string[] selectedAttributeIds = requestContent.Attributes.Select(x => x.Id.ToString()).ToArray();
                    var validAttributeIds = datasetAttributeIds.Any() ? selectedAttributeIds.Intersect(datasetAttributeIds) : selectedAttributeIds;
                    var validAttributes = requestContent.Attributes.Where(x => validAttributeIds.Contains(x.Id.ToString()));
                    foreach (var attribute in validAttributes)
                    {
                        string timeGrain = string.Equals(attribute.TimeInterval, "none", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : attribute.TimeInterval;
                        var requestItem = new SeriesRequestItem(requestContent.SubscriptionId,
                                                            requestContent.ProjectId,
                                                            Default.SERIES,
                                                            requestContent.AssetId,
                                                            new string[] { attribute.Id.ToString() },
                                                            GetStartDateFollowTimeGrain(command.FromDateUtcInMilliseconds, timeGrain, command.TimeZoneName),
                                                            command.ToDateUtcInMilliseconds,
                                                            timeGrain,
                                                            attribute.AggregationType,
                                                            attribute.Interpolated,
                                                            true);
                        requestItemList.Add(requestItem);
                    }

                    var payloadItem = new SeriesRequest(requestItemList)
                    {
                        TimezoneOffSet = DateTimeExtension.GetTimeZoneOffset(command.TimeZoneName),
                        Quality = SignalQualityCode.GOOD
                    };

                    var payloadJson = JsonConvert.SerializeObject(payloadItem);
                    response = await httpClient.PostAsync(
                        url,
                        new StringContent(payloadJson, Encoding.UTF8, MediaType.APPLICATION_JSON)
                    );
                }
                else return result;

                response.EnsureSuccessStatusCode();
                responseContent = await response.Content.ReadAsByteArrayAsync();

                builder.SetTimeZoneName(command.TimeZoneName);
                builder.SetDateTimeFormat(command.DateTimeFormat);
                builder.SetRequestContent(requestContent);
                builder.SetResponseContent(responseContent);
                builder.SetDataSetContent(dataSetContent);

                var dataTableResult = builder.BuildDataArray().BuildDataTable();

                _logger.LogDebug($"AmDataSourceHandler - dataTableResult - {dataTableResult.ToJson()}");
                result.Add(new DataTableResult(dataSetContent.DataSetName, dataTableResult));

                return result;
            }
            catch (HttpRequestException)
            {
                throw new SystemCallServiceException(detailCode: Message.AM_CALL_FAILED);
            }
        }

        /// <summary>
        /// read all column names in a dataset from xml template file
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="datasetName"></param>
        /// <returns></returns>
        private string[] GetDataSetColumnNames(Stream templateFile, string datasetName)
        {
            string[] columns = Array.Empty<string>();
            templateFile.Position = 0;
            var templateFileXml = XElement.Load(templateFile);
            templateFileXml.RemoveAllNamespaces();
            return templateFileXml.Descendants("ReportItems").Descendants("Chart").Where(x => x.Element("DataSetName").Value.ToLower() == datasetName.ToLower()).Descendants("ChartSeries").Select(x => x.Attribute("Name").Value.ToLower()).ToArray();
        }

        private long GetStartDateFollowTimeGrain(long startDateInMilisecond, string timeGrain, string timezoneName)
        {
            if (startDateInMilisecond == 0)
            {
                return 0;
            }

            if (string.IsNullOrWhiteSpace(timeGrain))
            {
                return startDateInMilisecond;
            }

            DateTime startDateInDateTimeFormat = DateTimeOffset.FromUnixTimeMilliseconds(startDateInMilisecond).DateTime.ToLocalDateTime(timezoneName);

            if (timeGrain.Contains(TimeGrain.MINUTE) || timeGrain.Contains(TimeGrain.MINUTES))
            {
                var firstSecondOfMinute = DateTimeExtension.GetFirstSecondOfMinute(startDateInDateTimeFormat);
                var rounedMinutes = RoundMinutesInTimeGrain(firstSecondOfMinute, timeGrain);

                return new DateTimeOffset(rounedMinutes.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            if (timeGrain.Contains(TimeGrain.HOUR))
            {
                var firstMinuteOfHour = DateTimeExtension.GetFirstMinuteOfHour(startDateInDateTimeFormat);
                return new DateTimeOffset(firstMinuteOfHour.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            if (timeGrain.Contains(TimeGrain.DAY))
            {
                var firstHourOfDay = DateTimeExtension.GetFirstHourOfDay(startDateInDateTimeFormat);
                return new DateTimeOffset(firstHourOfDay.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            if (timeGrain.Contains(TimeGrain.WEEK))
            {
                var firstDayOfWeek = DateTimeExtension.GetFirstDayOfWeek(startDateInDateTimeFormat);
                return new DateTimeOffset(firstDayOfWeek.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            if (timeGrain.Contains(TimeGrain.MONTH))
            {
                var firstDateOfMonth = DateTimeExtension.GetFirstDayOfMonth(startDateInDateTimeFormat);
                return new DateTimeOffset(firstDateOfMonth.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            if (timeGrain.Contains(TimeGrain.YEAR))
            {
                var firstDateOfYear = DateTimeExtension.GetFirstDayOfYear(startDateInDateTimeFormat);
                return new DateTimeOffset(firstDateOfYear.ToUtcDateTime(timezoneName)).ToUnixTimeMilliseconds();
            }

            return startDateInMilisecond;
        }

        public DateTime RoundMinutesInTimeGrain(DateTime startDate, string timeGrain)
        {
            if (string.IsNullOrWhiteSpace(timeGrain) || startDate == DateTime.MinValue)
            {
                return startDate;
            }

            var minutesFromTimeGrain = timeGrain.Replace(TimeGrain.MINUTES, string.Empty);
            int.TryParse(minutesFromTimeGrain, out int minutesInIntFormat);
            if (minutesInIntFormat == 0)
            {
                return startDate;
            }

            return startDate.AddMinutes(-(startDate.Minute % minutesInIntFormat));
        }
    }

    public class RequestAmContent
    {
        public string TenantId { get; set; }
        public string ProjectId { get; set; }
        public string SubscriptionId { get; set; }
        public Guid AssetId { get; set; }
        public int? ProjectSequentialNumber { get; set; }
        public IEnumerable<AttributeContent> Attributes { get; set; }
        public string DataType { get; set; }
        public IEnumerable<DataSetAmMappingContent> DataSetMappings { get; set; }

        public RequestAmContent()
        {
            DataSetMappings = new List<DataSetAmMappingContent>();
            Attributes = new List<AttributeContent>();
        }
    }

    public class DataSetAmMappingContent
    {
        public string PropName { get; set; }
        public string DataSetName { get; set; }
    }

    public class AttributeContent
    {
        public Guid Id { get; set; }
        public string TimeInterval { get; set; }
        public string AggregationType { get; set; }
        public bool Interpolated { get; set; }
    }

    public class SeriesRequest
    {
        public IEnumerable<SeriesRequestItem> Assets { get; set; }
        public string TimezoneOffSet { get; set; }
        public int? Quality { get; set; }

        public SeriesRequest(IEnumerable<SeriesRequestItem> assets)
        {
            Assets = assets;
        }
    }

    public class SeriesRequestItem
    {
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public string RequestType { get; set; }
        public Guid AssetId { get; set; }
        public IEnumerable<string> AttributeIds { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public string Timegrain { get; set; }
        public string Aggregate { get; set; }
        public long TimeoutInSeconds { get; set; } = 5;
        public bool UseCustomTimeRange { get; set; }
        public string HoursTimeRange { get; set; } = "Custom";
        public string GapFillFunction { get; set; }

        public SeriesRequestItem(string subscriptionId, string projectId, string requestType, Guid assetId, IEnumerable<string> attributeIds, long start, long end, string timegrain, string aggregate, bool interpolated = false, bool useCustomTimeRange = false)
        {
            SubscriptionId = subscriptionId;
            ProjectId = projectId;
            RequestType = requestType;
            AssetId = assetId;
            AttributeIds = attributeIds;
            Start = start;
            End = end;
            Timegrain = timegrain;
            Aggregate = aggregate;
            UseCustomTimeRange = useCustomTimeRange;
            GapFillFunction = interpolated ? "time_bucket_gapfill" : "time_bucket";
        }
    }

    public class GetAssetByIdResponse
    {
        public string Id { get; set; }

        public IEnumerable<GetAssetByIdAttributeResponse> Attributes { get; set; }

        public GetAssetByIdResponse()
        {
            Attributes = new List<GetAssetByIdAttributeResponse>();
        }
    }

    public class GetAssetByIdAttributeResponse
    {
        public string Id { get; set; }
    }

    public class SnapshotSeriesResponse
    {
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public string Aggregate { get; set; }
        public string Timegrain { get; set; }
        public string RequestType { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public IEnumerable<AttributeReponse> Attributes { get; set; }

        public SnapshotSeriesResponse()
        {
            Attributes = new List<AttributeReponse>();
        }
    }

    public class AttributeReponse
    {
        public string AssetId { get; set; }
        public string AttributeId { get; set; }
        public string AssetName { get; set; }
        public string AttributeName { get; set; }
        public string AttributeNameNormalize { get; set; }
        public int? DecimalPlace { get; set; }
        public bool? ThousandSeparator { get; set; }
        public UmoResponse Uom { get; set; }
        public string DataType { get; set; }

        public IEnumerable<SeriesResponse> Series { get; set; }

        public AttributeReponse()
        {
            Series = new List<SeriesResponse>();
        }
    }

    public class SeriesResponse
    {
        public long Ts { get; set; }
        public string V { get; set; }
    }

    public class UmoResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
    }

    public class SnapshotFlatResponse
    {
        public string AHI_Timestamp { get; set; }
        public string AHI_AssetName { get; set; }
        public string AHI_AttributeName { get; set; }
        public string AHI_Value { get; set; }

        public SnapshotFlatResponse(DateTime? ts, string assetName, string attributeName, string v, string uomAbbreviation)
        {
            AHI_AssetName = assetName;
            AHI_AttributeName = attributeName;

            if (ts != null)
                AHI_Timestamp = ts.ToString();

            if (!string.IsNullOrEmpty(v))
                AHI_Value = $"{v}{(string.IsNullOrWhiteSpace(uomAbbreviation) ? string.Empty : $" {uomAbbreviation}")}";
        }
    }

    public class TimeseriesFlatResponse
    {
        public DateTime AHI_Timestamp { get; set; }
        public string AHI_AttributeNameNormalize { get; set; }
        public string AHI_Value { get; set; }
        public string AHI_DataType { get; set; }
        public string AHI_Uom { get; set; }

        public TimeseriesFlatResponse(DateTime ts, string attributeNameNormalize, string v, string dataType, string uomAbbreviation)
        {
            AHI_Timestamp = ts;
            AHI_AttributeNameNormalize = attributeNameNormalize;
            AHI_Value = v;
            AHI_DataType = dataType;
            AHI_Uom = uomAbbreviation;
        }
    }
}