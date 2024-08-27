CREATE TABLE schedule_executions(
    id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    schedule_id INT NOT NULL,
    state varchar(5) NOT NULL DEFAULT 'INIT',
    max_retry_count INT NOT NULL,
    retry_count INT NOT NULL DEFAULT 0,
    retry_job_id varchar(50),
    created_utc DATETIME2 NOT NULL,
    updated_utc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    execution_param text,
    CONSTRAINT pk_schedule_executions PRIMARY KEY(id),
    CONSTRAINT fk_schedule_executions_schedules FOREIGN KEY(schedule_id) REFERENCES schedules(id) ON DELETE CASCADE
);
GO