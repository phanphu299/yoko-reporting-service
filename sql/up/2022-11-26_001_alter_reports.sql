IF NOT EXISTS(SELECT 1 FROM SYS.COLUMNS 
          WHERE NAME = 'schedule_name'
          AND OBJECT_ID = OBJECT_ID('reports'))
BEGIN
    ALTER TABLE reports ADD schedule_name NVARCHAR(2048) NULL
END