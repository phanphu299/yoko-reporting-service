IF NOT EXISTS(SELECT 1 FROM sys.columns 
              WHERE name = 'type'
              AND object_id = object_id('schedules'))
BEGIN
    ALTER TABLE schedules ADD [type] VARCHAR(50) NULL
    ALTER TABLE schedules ADD CONSTRAINT fk_schedules_type FOREIGN KEY([type]) REFERENCES schedule_types([id])
END


ALTER TABLE schedules ALTER COLUMN template_id INT NULL

IF (OBJECT_ID('fk_schedules_template_id', 'F') IS NOT NULL)
ALTER TABLE [schedules] DROP CONSTRAINT fk_schedules_template_id
