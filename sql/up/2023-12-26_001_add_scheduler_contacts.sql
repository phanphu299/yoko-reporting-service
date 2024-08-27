-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE schedule_contacts(
    id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    schedule_id INT NOT NULL,
    object_id UNIQUEIDENTIFIER NOT NULL,
    object_type VARCHAR(20) NOT NULL,
    sequential_number int NOT NULL CONSTRAINT df_schedule_contacts_sequential_number DEFAULT 0,
    created_utc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	updated_utc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
	deleted BIT NOT NULL DEFAULT 0,
    CONSTRAINT pk_schedule_contacts PRIMARY KEY
    (
        id
    ),
    CONSTRAINT fk_schedule_contacts_schedules FOREIGN KEY(schedule_id) REFERENCES schedules(id) ON DELETE CASCADE,
    CONSTRAINT uq_schedule_contacts UNIQUE (
        schedule_id,
        object_id,
        object_type
    )
)