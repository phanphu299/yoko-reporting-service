create table reports (
  id int identity(1,1) not null,
  name nvarchar(255) null,
  overridden nvarchar(max) null,
  template_id int not null,
  from_date_utc datetime2 not null default getutcdate(),
  to_date_utc datetime2 not null default getutcdate(),
  file_url nvarchar(max) not null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_reports primary key(id),
  constraint fk_reports_template_id foreign key(template_id) references templates(id)
);
go