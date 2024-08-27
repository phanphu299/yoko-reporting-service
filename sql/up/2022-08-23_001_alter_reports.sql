if not exists(select 1 from sys.columns 
          where name = 'storage_id'
          and object_id = object_id('reports'))
begin
    alter table reports add storage_id int null
end