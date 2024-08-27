using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Extension;
using JsonConstant = AHI.Infrastructure.SharedKernel.Extension.Constant;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;

namespace Reporting.Function.Extension
{
    public static class StringExtension
    {
        public static JToken TryExtractJsonObject(this string jsonString)
        {
            JToken jToken = null;
            jToken = TryExtractJObject(jsonString);
            if (jToken == null)
                jToken = TryExtractJArray(jsonString);
            return jToken;
        }

        public static JObject TryExtractJObject(this string jsonString)
        {
            try
            {
                return JObject.Parse(jsonString);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public static JArray TryExtractJArray(this string jsonString)
        {
            try
            {
                return JArray.Parse(jsonString);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public static string ExtractAzureBlobName(this string fullFilePath, string contrainerName)
        {
            fullFilePath = fullFilePath.Split(contrainerName)[1];
            if (fullFilePath.StartsWith("/"))
            {
                fullFilePath = fullFilePath.Substring(1, fullFilePath.Length - 1);
            }
            return fullFilePath;
        }

        public static string RemoveFileToken(this string fileName)
        {
            var index = fileName?.IndexOf("?token=") ?? -1;
            return index < 0 ? fileName ?? string.Empty : fileName.Remove(index);
        }

        public static string JsonSerialize(this object value)
        {
            return Encoding.UTF8.GetString(JsonExtension.Serialize(value));
        }

        public static T JsonDeserialize<T>(this string jsonString)
        {
            using (var reader = new System.IO.StringReader(jsonString))
            using (var jsonReader = new Newtonsoft.Json.JsonTextReader(reader))
            {
                return JsonConstant.JsonSerializer.Deserialize<T>(jsonReader);
            }
        }
        public static bool TryParseUtcDatetime(this string dateTimeString, out DateTime dateTime)
        {
            dateTime = DateTime.MinValue;
            var format = JsonConstant.DefaultDateTimeFormat;
            var provider = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;

            bool success = DateTime.TryParseExact(dateTimeString, format, provider, style, out dateTime);
            if (!success)
                success = DateTime.TryParse(dateTimeString, provider, style, out dateTime);
            return success;
        }

        public static IEnumerable<string> SplitStringWithSpan(this string stringToSplit, char seperator = ',')
        {
            if (string.IsNullOrEmpty(stringToSplit))
                return Array.Empty<string>();

            ReadOnlySpan<char> span = stringToSplit.AsSpan();
            int nextSeperatorIndex = 0;
            int insertValAtIndex = 0;
            bool isLastLoop = false;
            List<string> result = new List<string>();
            while (!isLastLoop)
            {
                int indexStart = nextSeperatorIndex;
                nextSeperatorIndex = stringToSplit.IndexOf(seperator, indexStart);
                isLastLoop = (nextSeperatorIndex == -1);
                if (isLastLoop)
                {
                    nextSeperatorIndex = stringToSplit.Length;
                }
                ReadOnlySpan<char> slice = span.Slice(indexStart, nextSeperatorIndex - indexStart);
                string valParsed = slice.ToString();
                result.Add(valParsed);
                insertValAtIndex++;

                // skip the seperator for next iteration
                nextSeperatorIndex++;
            }

            return result;
        }

        public static string CombineData(this IEnumerable<string> data, int lengthLimit = 255)
        {
            if (data is null || !data.Any())
                return null;
            var joinData = string.Join(", ", data);
            var length = Math.Min(lengthLimit, joinData.Length);
            return joinData.Substring(0, length);
        }

        public static string ReplaceNonLetterOrDigit(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var sb = new StringBuilder();

            foreach (var character in input)
            {
                if (char.IsLetterOrDigit(character))
                {
                    sb.Append(character);
                }
                else
                {
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }
    }
}