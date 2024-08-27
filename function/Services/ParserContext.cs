using System.Collections.Generic;
using Reporting.Function.Service.Abstraction;

namespace Reporting.Function.Service
{
    public class ParserContext : IParserContext
    {
        private IDictionary<string, string> _formats;

        public ParserContext()
        {
            _formats = new Dictionary<string, string>();
        }

        public void SetContextFormat(string key, string format)
        {
            _formats[key] = format;
        }

        public string GetContextFormat(string key)
        {
            return _formats.TryGetValue(key, out var result) ? result : null;
        }
    }
}