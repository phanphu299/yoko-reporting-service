if not exists(select 1 from sys.columns 
          where name = 'schedule_id'
          and object_id = object_id('reports'))
begin
    alter table reports add schedule_id int null
end