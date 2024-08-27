using System.IO;

namespace Reporting.Function.Extension
{
    public static class StreamExtension
    {
        public static MemoryStream ToMemoryStream(this Stream sourceStream)
        {
            var ms = new MemoryStream();
            sourceStream.CopyTo(ms);
            return ms;
        }
    }
}