using System;
using System.IO;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.SharedKernel.Extension;

namespace Reporting.Application.Extension
{
    public static class StreamExtension
    {
        public static JObject TryExtractJObject(this Stream jsonStream)
        {
            try
            {
                var outputStream = new MemoryStream();
                jsonStream.CopyTo(outputStream);
                jsonStream.Position = 0;
                outputStream.Position = 0;
                return outputStream.ToArray().Deserialize<JObject>();
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public static JArray TryExtractJArray(this Stream jsonStream)
        {
            try
            {
                var outputStream = new MemoryStream();
                jsonStream.CopyTo(outputStream);
                jsonStream.Position = 0;
                outputStream.Position = 0;
                return outputStream.ToArray().Deserialize<JArray>();
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public static string ConvertToBase64(this Stream stream)
        {
            stream.Position = 0;
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            var temp = Convert.ToBase64String(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] ConvertToByteArray(this Stream stream)
        {
            stream.Position = 0;
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }
    }
}