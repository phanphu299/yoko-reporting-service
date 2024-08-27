insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'authentication', 'Authentication', 'Authentication', 'table', 0, 0
from schemas
where type = 'API_DATA_SOURCE';