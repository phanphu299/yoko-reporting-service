create table schedules (
  id int identity(1,1) not null,
  name nvarchar(2048) not null, -- cron_description
  template_id int not null,
  cron_expression_id uniqueidentifier null,
  is_switched_to_cron bit not null,
  cron nvarchar(999) not null,
  timezone_name nvarchar(1024) not null,
  [endpoint] nvarchar(2048) not null,
  [method] varchar(10) not null,
  additional_params nvarchar(max) null,
  [start] datetime2 null,
  [end] datetime2 null,
  job_id varchar(50) null,
  last_run_utc datetime2 null,
  created_utc datetime2 not null default getutcdate(),
  updated_utc datetime2 not null default getutcdate(),
  deleted bit not null default 0,
  constraint pk_schedules primary key(id),
  constraint fk_schedules_template_id foreign key(template_id) references templates(id)
);
go