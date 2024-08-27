create table template_details(
  id int identity(1,1) not null,
  template_id int not null,
  data_source_content nvarchar(max) null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_template_details primary key(id),
  constraint fk_template_details_template_id foreign key(template_id) references templates(id)
)
go