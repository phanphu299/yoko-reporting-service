-- API data source
insert into schemas(name, type)
values ('API data source', 'API_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'endpoint', 'Endpoint', 'Endpoint', 'text', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'method', 'Method', 'Method', 'combobox', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'headers', 'Headers', 'Headers', 'table', 0, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'body', 'Body', 'Body', 'textarea', 0, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'dataSets', 'Data sets', 'Data sets', 'table', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

-- SQL Server data source
insert into schemas(name, type)
values ('SQL Server', 'SQL_SERVER_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'connectionString', 'Connection string', 'Connection string', 'text', 1, 0
from schemas
where type = 'SQL_SERVER_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'dataSets', 'Data sets', 'Data sets', 'table', 1, 0
from schemas
where type = 'SQL_SERVER_DATA_SOURCE';

-- Postgre data source
insert into schemas(name, type)
values ('Postgres', 'POSTGRE_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'connectionString', 'Connection string', 'Connection string', 'text', 1, 0
from schemas
where type = 'POSTGRE_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'dataSets', 'Data sets', 'Data sets', 'table', 1, 0
from schemas
where type = 'POSTGRE_DATA_SOURCE';

-- Native storage space
insert into schemas(name, type)
values ('Native storage', 'NATIVE_STORAGE_SPACE');

-- Azure blob storage space
insert into schemas(name, type)
values ('Azure blob storage', 'AZURE_BLOB_STORAGE_SPACE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'accountName', 'Account name', 'Account name', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'accountKey', 'Account key', 'Account key', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'containerName', 'Container name', 'Container name', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'blobName', 'Blob name', 'Blob name', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';