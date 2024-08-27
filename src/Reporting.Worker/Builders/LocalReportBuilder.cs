using AHI.Infrastructure.Exception;
using Microsoft.Reporting.NETCore;
using Reporting.Application.Command.Model;
using Reporting.Application.Constant;
using Reporting.Application.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Reporting.Worker.Builder
{
    public class LocalReportBuilder
    {
        private DateTime _fromDate;
        private DateTime _toDate;
        private LocalReport _localReport;
        private SimpleTemplateByIdDto _template;
        private Stream _templateFile;
        private IEnumerable<DataTableResult> _dataTableResults;

        public LocalReportBuilder(DateTime fromDate, DateTime toDate)
        {
            _fromDate = fromDate;
            _toDate = toDate;
            _localReport = new LocalReport();
            _localReport.EnableExternalImages = true;
        }

        public void SetTemplate(SimpleTemplateByIdDto template)
        {
            _template = template;
        }

        public void SetTemplateFile(Stream templateFile)
        {
            templateFile.Position = 0;
            _templateFile = templateFile;
            _localReport.LoadReportDefinition(_templateFile);
        }

        public void SetDataTableResults(IEnumerable<DataTableResult> dataTableResults)
        {
            _dataTableResults = dataTableResults;
        }

        public LocalReportBuilder BuildDataTables()
        {
            var dataSouceNames = _localReport.GetDataSourceNames();
            foreach (var dataSourceName in dataSouceNames)
            {
                bool filled = false;
                foreach (var dataTableResult in _dataTableResults)
                {
                    if (dataTableResult.Name.ToLower() == dataSourceName.ToLower())
                    {
                        // remove existing one
                        var existingDataSource = _localReport.DataSources.FirstOrDefault(x => x.Name == dataSourceName);
                        if (existingDataSource != null)
                            _localReport.DataSources.Remove(existingDataSource);

                        // add new one
                        var dataSource = new ReportDataSource(dataSourceName, dataTableResult.Table);
                        _localReport.DataSources.Add(dataSource);

                        // filled with data
                        filled = true;
                    }
                }

                // add blank data
                if (!filled)
                    _localReport.DataSources.Add(new ReportDataSource(dataSourceName, new DataTable()));
            }
            return this;
        }

        public LocalReportBuilder BuildParams()
        {
            var param = _template.Default.TryGetCaseInSensitiveDictionary();
            param.Add(FieldName.FROM_DATE, _fromDate);
            param.Add(FieldName.TO_DATE, _toDate);
            foreach (var localParam in _localReport.GetParameters())
            {
                string value = string.Empty;
                if (param.ContainsKey(localParam.Name))
                    value = param[localParam.Name]?.ToString();
                _localReport.SetParameters(new ReportParameter(localParam.Name, !string.IsNullOrEmpty(value) ? value : DefaultValue.NOT_APPLICABLE));
            }
            return this;
        }

        public MemoryStream BuildReportFile()
        {
            try
            {
                byte[] file = _localReport.Render(_template.OutputType.Id);
                return new MemoryStream(file);
            }
            catch (System.Exception ex) when (ex is LocalProcessingException || ex is ArgumentOutOfRangeException)
            {
                throw new GenericProcessFailedException(detailCode: Message.LOCAL_REPORT_FILL_FAILED, innerException: ex);
            }
        }
    }
}