DECLARE @typeId VARCHAR(50) = 'REPORT_AND_SEND';
DECLARE @typeName VARCHAR(50) = 'Report & Send';
DECLARE @localeKey VARCHAR(50) = 'SCHEDULE.TYPE.REPORT_AND_SEND';

if not exists(select 1 from schedule_types where id = @typeId)
begin
    insert into schedule_types(id, name, localization_key)
    values (@typeId, @typeName, @localeKey)
end
else
begin
    update schedule_types set name = @typeName, localization_key = @localeKey
    where id = @typeId
end

SET @typeId = 'SEND';
SET @typeName = 'Send';
SET @localeKey = 'SCHEDULE.TYPE.SEND';

if not exists(select 1 from schedule_types where id = @typeId)
begin
    insert into schedule_types(id, name, localization_key)
    values (@typeId, @typeName, @localeKey)
end
else
begin
    update schedule_types set name = @typeName, localization_key = @localeKey
    where id = @typeId
end
