namespace Device.Application.Constant
{
    public static class Privileges
    {
        public static class Report
        {
            public const string ENTITY_NAME = "report";

            public static class Rights
            {
                public const string READ_REPORT = "read_report";
                public const string WRITE_REPORT = "write_report";
                public const string DELETE_REPORT = "delete_report";
            }

            public static class FullRights
            {
                public const string READ_REPORT = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report/read_report";
                public const string WRITE_REPORT = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report/write_report";
                public const string DELETE_REPORT = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report/delete_report";
            }
        }

        public static class ReportTemplate
        {
            public const string ENTITY_NAME = "report_template";

            public static class Rights
            {
                public const string READ_REPORT_TEMPLATE = "read_report_template";
                public const string WRITE_REPORT_TEMPLATE = "write_report_template";
                public const string DELETE_REPORT_TEMPLATE = "delete_report_template";
            }

            public static class FullRights
            {
                public const string READ_REPORT_TEMPLATE = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report_template/read_report_template";
                public const string WRITE_REPORT_TEMPLATE = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report_template/write_report_template";
                public const string DELETE_REPORT_TEMPLATE = "ea8f57b2-f183-4acc-88b0-249ecb59286e/report_template/delete_report_template";
            }
        }
    }
}