alter table templates drop column data_source_type
alter table templates drop column storage_space_type
alter table templates drop column output_type_code
alter table templates drop column storage_space_content
alter table templates drop column scheduler_id
alter table templates drop column last_run_utc
alter table templates add output_type_id varchar(255) null
alter table templates add storage_id int null
alter table templates add dataset_count int null
alter table templates add constraint fk_templates_storage_id foreign key(storage_id) references storages(id)
alter table templates add constraint fk_templates_output_type_id foreign key(output_type_id) references output_types(id)