CREATE TABLE entity_tags (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    tag_id BIGINT NOT NULL,
    entity_id_varchar VARCHAR(100) NULL,
    entity_id_int INT NULL,
    entity_id_long BIGINT NULL,
    entity_id_uuid UNIQUEIDENTIFIER NULL,
    entity_type VARCHAR(100) NOT NULL
);