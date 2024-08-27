-- storage type
insert into storage_types(id, name, can_read)
values ('NATIVE_STORAGE_SPACE', 'Local Storage', 0)

insert into storage_types(id, name)
values ('AZURE_BLOB_STORAGE_SPACE', 'Azure Blob Storage')

-- storage
insert into storages(name, type_id, can_edit, can_delete)
select 'AHS Storage', id, 0, 0
from storage_types where id = 'NATIVE_STORAGE_SPACE'