alter table template_details add name nvarchar(255) null
alter table template_details add data_source_type_id varchar(255) null
alter table template_details add constraint fk_template_details_data_source_type_id foreign key(data_source_type_id) references data_source_types(id)