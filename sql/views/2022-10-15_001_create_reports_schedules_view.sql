CREATE OR ALTER VIEW v_report_schedules
AS
SELECT schedule_id AS id, schedule_name AS name, reports.created_by, reports.resource_path, schedules.cron_description, reports.template_id, MAX(reports.created_utc) as last_run_utc, schedules.created_utc, schedules.deleted AS has_deleted_schedule
FROM reports WITH(NOLOCK) JOIN schedules WITH(NOLOCK)
ON schedules.id = reports.schedule_id 
WHERE reports.deleted = 0 
GROUP BY schedule_id, reports.template_id, schedule_name, reports.created_by, reports.resource_path, schedules.cron_description, last_run_utc, schedules.created_utc, schedules.deleted 