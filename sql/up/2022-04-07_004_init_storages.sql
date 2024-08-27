create table storage_types (
  id varchar(255) not null,
  name nvarchar(255) null,
  can_read bit not null default 1,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_storage_types primary key(id)
);
go

create table storages (
  id int identity(1,1) not null,
  name nvarchar(255) null,
  type_id varchar(255) not null,
  content nvarchar(max) null,
  can_edit bit default 1 not null,
  can_delete bit default 1 not null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_storages primary key(id),
  constraint fk_storages_type_id foreign key(type_id) references storage_types(id)
);
go