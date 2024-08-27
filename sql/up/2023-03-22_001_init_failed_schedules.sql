create table failed_schedules (
  id int identity(1,1) not null,
  schedule_name nvarchar(2048) null,
  schedule_id int not null,
  job_id varchar(50) not null,
  timezone_name varchar(50) not null,
  execution_time bigint null,
  next_execution_time bigint null,
  previous_execution_time bigint null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_failed_schedules primary key(id),
  constraint fk_failed_schedules_schedules_id foreign key(schedule_id) references schedules(id) on delete cascade
);
go
