using System.Collections.Generic;

namespace Reporting.Application.Constant
{
    public static class OutputType
    {
        private static readonly IDictionary<string, string> _outputTypeMapping = new Dictionary<string, string>() {
            { PDF, PDF_EXTENSION },
            { WORD, WORD_EXTENSION },
            { EXCEL, EXCEL_EXTENSION }
        };

        public const string PDF_EXTENSION = ".pdf";
        public const string WORD_EXTENSION = ".docx";
        public const string EXCEL_EXTENSION = ".xlsx";

        public const string PDF = "PDF";
        public const string WORD = "WORDOPENXML";
        public const string EXCEL = "EXCELOPENXML";

        public static bool CheckOutputType(string outputType) => _outputTypeMapping.ContainsKey(outputType);
        public static string GetOutputExtension(string outputType) => _outputTypeMapping[outputType];
    }
}