create table templates(
  id int identity(1,1) not null,
  name nvarchar(255) not null,
  template_file_url nvarchar(2048) not null,
  [default] nvarchar(max) null,
  data_source_type varchar(255) not null,
  storage_space_type varchar(255) not null,
  output_type_code varchar(50) not null,
  storage_space_content nvarchar(max) null,
  scheduler_id varchar(255) null,
  last_run_utc datetime2 null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_templates primary key(id)
)
go