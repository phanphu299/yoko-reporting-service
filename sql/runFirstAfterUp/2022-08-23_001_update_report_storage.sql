update r
    set r.storage_id = t.storage_id
from reports r
    join templates t with(nolock) on t.id = r.template_id