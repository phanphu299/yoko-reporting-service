insert into schemas(name, type)
values ('Asset data source', 'ASSET_MANAGEMENT_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'assetIdPath', 'Elements Id Path', 'Elements Id Path', 'text', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'assetNamePath', 'Elements Name Path', 'Elements Name Path', 'text', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'attributeIds', 'Attributes', 'Attributes', 'text', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'datasetMappings', 'Dataset Mappings', 'Dataset Mappings', 'table', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';