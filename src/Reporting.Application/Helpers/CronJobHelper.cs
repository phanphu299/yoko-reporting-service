using System.Collections.Generic;
using CronExpressionDescriptor;

namespace Scheduler.Application.Helper
{
    public static class CronJobHelper
    {
        public static bool IsValidCronExpression(string expression)
        {
            return Quartz.CronExpression.IsValidExpression(expression);
        }

        public static string GenerateDescription(string expression)
        {
            expression = GetOverrideCronExpression(expression);
            return ExpressionDescriptor.GetDescription(expression);
        }

        /// <summary>
        /// Quartz day of week is not standard, the purpose of the function is to modify portion day of week in the cron
        /// so that the description can be generated properly
        /// *NOTE: Only for generating description purpose
        /// - Quartz:   1-7 (SUN-SAT)
        /// - Standard: 0-6 (SUN-SAT)
        /// </summary>
        private static string GetOverrideCronExpression(string expression)
        {
            var standardDayOfWeekMapping = new Dictionary<string, string>()
            {
                ["1"] = "0",
                ["2"] = "1",
                ["3"] = "2",
                ["4"] = "3",
                ["5"] = "4",
                ["6"] = "5",
                ["7"] = "6"
            };

            var spitExpression = expression.Split(" ");
            var dayOfWeek = spitExpression[5];

            if (!string.IsNullOrEmpty(dayOfWeek))
            {
                var splitDayOfWeek = dayOfWeek.Split(",");

                for (var i = 0; i < splitDayOfWeek.Length; i++)
                {
                    if (standardDayOfWeekMapping.ContainsKey(splitDayOfWeek[i]))
                    {
                        splitDayOfWeek[i] = standardDayOfWeekMapping[(splitDayOfWeek[i])];
                        continue;
                    }
                }

                spitExpression[5] = string.Join(",", splitDayOfWeek);

                expression = string.Join(" ", spitExpression);
            }

            return expression;
        }
    }
}