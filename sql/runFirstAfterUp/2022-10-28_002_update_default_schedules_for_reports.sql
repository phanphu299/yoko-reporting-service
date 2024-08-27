DECLARE @scheduleId INT;

SET @scheduleId = (SELECT TOP 1 id FROM schedules WITH(NOLOCK) WHERE NAME = 'Default') 

UPDATE reports SET schedule_id = @scheduleId WHERE schedule_id IS NULL