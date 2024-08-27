insert into schemas(name, type)
values ('Asset data source', 'ASSET_MANAGEMENT_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'assetId', 'Elements', 'Elements', 'text', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'connectionId', 'Connection', 'Connection', 'combobox', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'dataType', 'Data Type', 'Data Type', 'combobox', 1, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'timeInterval', 'Time Interval', 'Time Interval', 'combobox', 0, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'aggregationType', 'Aggregation Type', 'Aggregation Type', 'combobox', 0, 0
from schemas
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';