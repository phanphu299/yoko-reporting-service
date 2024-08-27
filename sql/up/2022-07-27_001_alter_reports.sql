if not exists(select 1 from sys.columns 
          where name = 'output_type_id'
          and object_id = object_id('reports'))
begin
    alter table reports add output_type_id varchar(255) null
end

-- drop no need column
if exists(select 1 from sys.columns 
          where name = 'download_url'
          and object_id = object_id('reports'))
begin
    declare @default_cs_name nvarchar(100) =
    (
        select object_name([default_object_id]) from sys.columns
        where [object_id] = object_id('reports') and [name] = 'download_url'
    );
    
    if (@default_cs_name is not null)
    begin
        exec('alter table reports drop constraint ' + @default_cs_name)
        alter table reports drop column download_url
    end
end

-- cause the template can be deleted, save its name for viewing reports purpose
if not exists(select 1 from sys.columns 
          where name = 'template_name'
          and object_id = object_id('reports'))
begin
    alter table reports add template_name nvarchar(255) null
end