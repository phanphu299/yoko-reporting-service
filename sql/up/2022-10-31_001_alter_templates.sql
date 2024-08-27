if exists(select 1 from sys.columns 
          where name = 'dataset_count'
          and object_id = object_id('templates'))
begin
    alter table templates drop column dataset_count 
end