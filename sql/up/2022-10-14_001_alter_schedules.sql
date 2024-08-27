if not exists(select 1 from sys.columns 
          where name = 'cron_description'
          and object_id = object_id('schedules'))
begin
    alter table schedules add cron_description nvarchar(1024) null
end