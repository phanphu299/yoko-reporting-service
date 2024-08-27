create table schemas(
  id int identity(1,1) not null,
  type varchar(255) not null,
  name nvarchar(255) not null,
  created_utc datetime not null default getutcdate(),
  updated_utc datetime not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_schemas primary key(id)
);
go

create table schema_details(
  id int identity(1,1) not null,
  [key] varchar(255) not null,
  name nvarchar(255) not null,
  place_holder nvarchar(2048) null,
  data_type nvarchar(50) not null,
  is_required bit not null default 0,
  is_readonly bit not null default 0,
  schema_id int not null,
  created_utc datetime not null default getutcdate(),
  updated_utc datetime not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_schema_details primary key(id),
  constraint fk_schema_details_schema_id foreign key(schema_id) references schemas(id)
);
go