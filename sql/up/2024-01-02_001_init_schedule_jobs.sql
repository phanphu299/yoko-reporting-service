CREATE TABLE schedule_jobs (
  id INT IDENTITY(1,1) NOT NULL,
  schedule_id INT NOT NULL,
  job_id INT NOT NULL,
  CONSTRAINT pk_schedule_jobs PRIMARY KEY(id),
  CONSTRAINT fk_schedule_jobs_schedules FOREIGN KEY(schedule_id) REFERENCES schedules([id]),  
  CONSTRAINT fk_schedule_jobs_jobs FOREIGN KEY(job_id) REFERENCES schedules([id])
);
GO