delete from schema_details where schema_id = (select id from schemas where type = 'AZURE_BLOB_STORAGE_SPACE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'connectionString', 'Connection String', 'Connection String', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'containerName', 'Container Name', 'Container Name', 'text', 1, 0
from schemas
where type = 'AZURE_BLOB_STORAGE_SPACE';