-- reports
IF EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
	DROP INDEX idx_reports_resourcepath_createdby ON reports;
    ALTER TABLE reports ALTER COLUMN created_by nvarchar(255);
	CREATE INDEX idx_reports_resourcepath_createdby ON reports(resource_path, created_by);
END

-- templates
IF EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('templates'))
BEGIN
	DROP INDEX idx_templates_resourcepath_createdby ON templates;
    ALTER TABLE templates ALTER COLUMN created_by nvarchar(255)
	CREATE INDEX idx_templates_resourcepath_createdby ON templates(resource_path, created_by);
END

-- schedules
IF EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('schedules'))
BEGIN
	DROP INDEX idx_schedules_resourcepath_createdby ON schedules;
    ALTER TABLE schedules ALTER COLUMN created_by nvarchar(255)
	CREATE INDEX idx_schedules_resourcepath_createdby ON schedules(resource_path, created_by);
END

