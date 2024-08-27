CREATE TABLE schedule_templates (
  template_id INT NOT NULL,
  schedule_id INT NOT NULL,
  CONSTRAINT pk_schedule_templates PRIMARY KEY(template_id, schedule_id),
  CONSTRAINT fk_schedule_templates_templates FOREIGN KEY(template_id) REFERENCES templates([id]),
  CONSTRAINT fk_schedule_templates_schedules FOREIGN KEY(schedule_id) REFERENCES schedules([id])
);
GO