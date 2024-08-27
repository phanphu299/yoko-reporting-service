IF NOT EXISTS(SELECT 1 FROM sys.columns 
              WHERE name = 'period'
              AND object_id = object_id('schedules'))
BEGIN
    ALTER TABLE schedules ADD [period] VARCHAR(50) NULL
END
