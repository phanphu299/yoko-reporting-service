-- API
if not exists(select 1 from data_source_types where id = 'API_DATA_SOURCE' and name = 'API')
begin
    insert into data_source_types(id, name)
    values ('API_DATA_SOURCE', 'API')
end
else
begin
    update data_source_types set name = 'API' where id = 'API_DATA_SOURCE'
end

-- Asset Management
if not exists(select 1 from data_source_types where id = 'ASSET_MANAGEMENT_DATA_SOURCE' and name = 'Asset Management')
begin
    insert into data_source_types(id, name)
    values ('ASSET_MANAGEMENT_DATA_SOURCE', 'Asset Management')
end
else
begin
    update data_source_types set name = 'Asset Management' where id = 'ASSET_MANAGEMENT_DATA_SOURCE'
end

-- SQL Server
if not exists(select 1 from data_source_types where id = 'SQL_SERVER_DATA_SOURCE' and name = 'SQL Server')
begin
    insert into data_source_types(id, name)
    values ('SQL_SERVER_DATA_SOURCE', 'SQL Server')
end
else
begin
    update data_source_types set name = 'SQL Server' where id = 'SQL_SERVER_DATA_SOURCE'
end

-- Postgre
if not exists(select 1 from data_source_types where id = 'POSTGRE_DATA_SOURCE' and name = 'Postgre')
begin
    insert into data_source_types(id, name)
    values ('POSTGRE_DATA_SOURCE', 'Postgre')
end
else
begin
    update data_source_types set name = 'Postgre' where id = 'POSTGRE_DATA_SOURCE'
end