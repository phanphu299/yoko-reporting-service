delete from schema_details where schema_id in (select id from schemas WITH (NOLOCK) where type = 'ASSET_MANAGEMENT_DATA_SOURCE');
delete from schemas where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schemas(name, type)
values ('Asset data source', 'ASSET_MANAGEMENT_DATA_SOURCE');

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'tenantId', 'Tenant', 'Tenant', 'combobox', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'subscriptionId', 'Subscription', 'Subscription', 'combobox', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'projectId', 'Project', 'Project', 'combobox', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'assetId', 'Elements', 'Elements', 'text', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'attributes', 'Attributes', 'Attributes', 'table', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'dataType', 'Data Type', 'Data Type', 'combobox', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'timeInterval', 'Time Interval', 'Time Interval', 'combobox', 0, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'aggregationType', 'Aggregation Type', 'Aggregation Type', 'combobox', 0, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';

insert into schema_details(schema_id, [key], name, place_holder, data_type, is_required, is_readonly)
select id, 'datasetMappings', 'Dataset Mappings', 'Dataset Mappings', 'table', 1, 0
from schemas WITH (NOLOCK)
where type = 'ASSET_MANAGEMENT_DATA_SOURCE';