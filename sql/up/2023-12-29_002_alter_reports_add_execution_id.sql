ALTER TABLE reports ADD schedule_execution_id UNIQUEIDENTIFIER;
GO
ALTER TABLE reports ADD CONSTRAINT fk_reports_schedule_executions FOREIGN KEY(schedule_execution_id) REFERENCES schedule_executions(id) ON DELETE SET NULL;
GO
