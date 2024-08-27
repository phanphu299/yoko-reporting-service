IF NOT EXISTS(SELECT 1 FROM sys.columns 
              WHERE name = 'localization_key'
              AND object_id = object_id('schedule_types'))
BEGIN
    ALTER TABLE schedule_types ADD [localization_key] VARCHAR(50) NULL
END
