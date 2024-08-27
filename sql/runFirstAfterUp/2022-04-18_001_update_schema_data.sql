delete from schema_details where schema_id = (select id from schemas where type = 'API_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'endpoint', 'Endpoint', 'Endpoint', 'text', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'method', 'Method', 'Method', 'combobox', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'query', 'Query Parametters', 'Query Parametters', 'text', 0, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'headers', 'Headers', 'Headers', 'textarea', 1, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'body', 'Body', 'Body', 'textarea', 0, 0
from schemas
where type = 'API_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'datasetMappings', 'Dataset Mappings', 'Dataset Mappings', 'table', 1, 0
from schemas
where type = 'API_DATA_SOURCE';