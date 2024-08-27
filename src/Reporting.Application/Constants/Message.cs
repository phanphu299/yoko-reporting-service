namespace Reporting.Application.Constant
{
    /// <summary>
    /// message code with pattern
    /// error:      <Service domain>.<Entity>.<Action>.<Message>
    /// validation: <Service domain>.<Entity>.<Field>.<ValidationType>
    /// </summary>
    public class Message
    {
        /* error */
        public const string LOCAL_REPORT_FILL_FAILED = "REPORT.LOCAL_REPORT.FILL.FAILED";
        public const string AZURE_BLOB_UPLOAD_FAILED = "REPORT.AZURE_BLOB.UPLOAD.FAILED";
        public const string API_CALL_FAILED = "REPORT.API.CALL.FAILED";
        public const string AM_CALL_FAILED = "REPORT.AM.CALL.FAILED";
        public const string NATIVE_STORAGE_UPLOAD_FAILED = "REPORT.NATIVE_STORAGE.UPLOAD.FAILED";
        public const string NATIVE_STORAGE_DOWNLOAD_FAILED = "REPORT.NATIVE_STORAGE.DOWNLOAD.FAILED";
        public const string NATIVE_STORAGE_GET_DOWNLOAD_TOKEN_FAILED = "REPORT.NATIVE_STORAGE.GET_DOWNLOAD_TOKEN.FAILED";
        public const string SQL_SERVER_QUERY_FAILED = "REPORT.SQL_SERVER.QUERY.FAILED";
        public const string POSTGRE_QUERY_FAILED = "REPORT.POSTGRE.QUERY.FAILED";
        public const string DATASOURCE_IS_DELETED = "REPORT.DATASOURCE_IS_DELETED";
        public const string SCHEDULE_HAS_LINKED_SEND_SCHEDULES = "REPORT.SCHEDULE.DELETE.HAS_LINKED_SEND_SCHEDULES";

        /* validation */
        public const string TEMPLATE_BEING_USED = "REPORT.TEMPLATE.ID.BEING_USED";
        public const string SCHEDULE_TYPE_CANNOT_CHANGED = "SCHEDULE.TYPE.CANNOT_CHANGED";
        public const string STORAGE_BEING_USED = "REPORT.STORAGE.ID.BEING_USED";
        public const string WARNING_VALIDATION_SOME_CONTACTS_OR_CONTACT_GROUPS_DELETED = "WARNING.ENTITY.VALIDATION.SOME_CONTACTS_OR_CONTACT_GROUPS_DELETED";
    }
}