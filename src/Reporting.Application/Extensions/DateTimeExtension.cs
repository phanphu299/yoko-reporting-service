using System;
using TimeZoneConverter;

namespace Reporting.Application.Extension
{
    public static class DateTimeExtension
    {
        public const string DEFAULT_DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss.fff";
        public const string DEFAULT_DATETIME_OFFSET = "00:00";
        public const string TIMESTAMP_FORMAT = "yyyyMMddHHmmss";
        public const string LONG_TIMESTAMP_FORMAT = "yyyyMMddHHmmssfff";
        public const string SHORT_TIMESTAMP_FORMAT = "yyyyMMdd";
        public const string DEFAULT_TIMEZONE_NAME = "Singapore Standard Time";

        public static DateTime ToUtcDateTime(this long dateTimeUtcInMilliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeUtcInMilliseconds).UtcDateTime;
        }

        public static string ToUtcDateTimeInFormat(this long dateTimeUtcInMilliseconds, string dateTimeFormat)
        {
            return ToUtcDateTime(dateTimeUtcInMilliseconds).ToString(dateTimeFormat);
        }

        public static string ToLocalDateTimeInFormat(this long dateTimeUtcInMilliseconds, string timeZoneName, string dateTimeFormat)
        {
            if (dateTimeUtcInMilliseconds == 0)
            {
                return string.Empty;
            }

            return ToUtcDateTime(dateTimeUtcInMilliseconds).ToLocalDateTime(timeZoneName).ToString(dateTimeFormat);
        }

        public static DateTime ToLocalDateTime(this long dateTimeUtcInMilliseconds, string timeZoneName)
        {
            if (dateTimeUtcInMilliseconds == 0)
            {
                return DateTime.MinValue;
            }

            return ToUtcDateTime(dateTimeUtcInMilliseconds).ToLocalDateTime(timeZoneName);
        }

        public static DateTime ToLocalDateTime(this DateTime dateTime, string timeZoneName)
        {
            dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            var timeZoneInfo = GetTimeZoneInfo(timeZoneName);
            return TimeZoneInfo.ConvertTime(dateTime, timeZoneInfo);
        }

        public static DateTime ToUtcDateTime(this DateTime dateTime, string timeZoneName)
        {
            var timeZoneInfo = GetTimeZoneInfo(timeZoneName);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);
        }

        /// <summary>
        /// the function will get timezoneinfo based on timezone of windows system
        /// </summary>
        /// <param name="timeZoneName"></param>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneInfo(this string timeZoneName)
        {
            try
            {
                if (!string.IsNullOrEmpty(timeZoneName))
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                return TimeZoneInfo.Utc;
            }
            // the exception is throw which means given timeZoneName is invalid or the code is running on linux system
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    string linuxTimeZoneName = TZConvert.WindowsToIana(timeZoneName);
                    return TimeZoneInfo.FindSystemTimeZoneById(linuxTimeZoneName);
                }
                catch (InvalidTimeZoneException)
                {
                    // invalid timeZoneName
                    return null;
                }
            }
        }

        /// <summary>
        /// get first second of minute
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstSecondOfMinute(DateTime dateTime)
        {
            return dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute);
        }

        /// <summary>
        /// get first minute of hour
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstMinuteOfHour(DateTime dateTime)
        {
            return dateTime.Date.AddHours(dateTime.Hour);
        }

        /// <summary>
        /// get first hour of day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstHourOfDay(DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// get first day of weeek
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// get first day of month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// get first day of year
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1);
        }

        public static string GetTimeZoneOffset(string timezoneName)
        {
            if (string.IsNullOrWhiteSpace(timezoneName))
            {
                return string.Empty;
            }

            var timezoneInfo = GetTimeZoneInfo(timezoneName);
            if (timezoneInfo == null)
            {
                return string.Empty;
            }

            var offset = timezoneInfo.GetUtcOffset(DateTime.UtcNow);
            return string.Format("{0}{1:00}:{2:00}", offset.Hours > 0 ? "+" : string.Empty, offset.Hours, offset.Minutes);
        }
    }
}