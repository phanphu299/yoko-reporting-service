IF NOT EXISTS(SELECT 1 FROM SYS.INDEXES 
          WHERE NAME = 'idx_reports_templateid_createdutc'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
    CREATE INDEX idx_reports_templateid_createdutc ON reports(template_id, created_utc)
    WHERE ([deleted]=(0));
END
