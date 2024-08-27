-- reports
IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'resource_path'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
    ALTER TABLE reports ADD resource_path varchar(1024);
END

IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
    ALTER TABLE reports ADD created_by varchar(50)
END

IF NOT EXISTS(SELECT 1 FROM SYS.INDEXES 
          WHERE NAME = 'idx_reports_resourcepath_createdby'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
    CREATE INDEX idx_reports_resourcepath_createdby ON reports(resource_path, created_by);
END

-- templates
IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'resource_path'
          AND OBJECT_ID = OBJECT_ID('templates'))
BEGIN
    ALTER TABLE templates ADD resource_path varchar(1024);
END

IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('templates'))
BEGIN
    ALTER TABLE templates ADD created_by varchar(50)
END

IF NOT EXISTS(SELECT 1 FROM SYS.INDEXES 
          WHERE NAME = 'idx_templates_resourcepath_createdby'
          AND OBJECT_ID = OBJECT_ID('templates'))
BEGIN
    CREATE INDEX idx_templates_resourcepath_createdby ON templates(resource_path, created_by);
END

-- schedules
IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'resource_path'
          AND OBJECT_ID = OBJECT_ID('schedules'))
BEGIN
    ALTER TABLE schedules ADD resource_path varchar(1024);
END

IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'created_by'
          AND OBJECT_ID = OBJECT_ID('schedules'))
BEGIN
    ALTER TABLE schedules ADD created_by varchar(50)
END

IF NOT EXISTS(SELECT 1 FROM SYS.INDEXES 
          WHERE NAME = 'idx_schedules_resourcepath_createdby'
          AND OBJECT_ID = OBJECT_ID('schedules'))
BEGIN
    CREATE INDEX idx_schedules_resourcepath_createdby ON schedules(resource_path, created_by);
END

