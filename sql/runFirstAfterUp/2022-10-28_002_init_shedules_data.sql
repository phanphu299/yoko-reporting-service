IF NOT EXISTS(SELECT 1 FROM schedules WITH(NOLOCK)
              WHERE NAME = 'Default')
BEGIN
	DECLARE @maxInt INT = 2147483647;
    DECLARE @templateId INT;
    SET @templateId = (SELECT top 1 id FROM templates WITH(NOLOCK) WHERE NAME = 'Default') 
	SET IDENTITY_INSERT schedules ON;
    INSERT INTO schedules(id, NAME, template_id, is_switched_to_cron, cron, timezone_NAME, endpoint, method, deleted)
    VALUES (@maxInt, 'Default', @templateId, 0, '', '','','', 1);
	SET IDENTITY_INSERT schedules OFF;
    DBCC CHECKIDENT (schedules, RESEED, 0);
END