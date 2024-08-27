namespace Reporting.Function.Service.Abstraction
{
    public interface IParserContext
    {
        void SetContextFormat(string key, string format);
        string GetContextFormat(string key);
    }
}