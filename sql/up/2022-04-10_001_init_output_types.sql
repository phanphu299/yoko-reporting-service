create table output_types (
  id varchar(255) not null,
  name nvarchar(255) not null,
  extension varchar(255) not null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_output_types primary key(id)
);
go