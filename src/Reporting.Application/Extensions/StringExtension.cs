using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Reporting.Application.Extension
{
    public static class StringExtension
    {
        /// <summary>
        /// The method will extract first array in the json
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static JArray TryExtractFirstJArray(this string jsonString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                    return null;
                jsonString = jsonString.Trim();
                if (jsonString.StartsWith("[") && jsonString.EndsWith("]"))
                {
                    return JArray.Parse(jsonString);
                }
                else
                {
                    var jObject = JObject.Parse(jsonString);
                    var jArray = jObject.Descendants().Where(x => x is JArray).FirstOrDefault();
                    if (jArray != null)
                    {
                        return jArray as JArray;
                    }
                    return null;
                }
            }
            catch (Newtonsoft.Json.JsonReaderException)
            {
                return null;
            }
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

        public static IDictionary<string, object> TryGetCaseInSensitiveDictionary(this IDictionary<string, object> dic)
        {
            var newDic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (dic == null)
                return newDic;
            foreach (var d in dic)
            {
                newDic.Add(d.Key, d.Value);
            }
            return newDic;
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

        public static string ExtractNumber(this string text)
        {
            if (decimal.TryParse(text, out decimal num))
            {
                return num.ToString();
            }
            return text;
        }

        public static bool IsNumber(this string text)
        {
            if (decimal.TryParse(text, out decimal num))
            {
                return true;
            }
            return false;
        }

        public static string ExtractPeriodValue(this string input)
        {
            return input.Substring(2, input.Length - 3);
        }
    }
}