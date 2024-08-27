CREATE OR ALTER VIEW v_report_templates
AS
SELECT ROW_NUMBER() OVER( ORDER BY r.created_utc DESC ) AS id,
	r.template_id,
	r.template_name AS name,
	r.created_by,
	r.resource_path,
	r.created_utc,
	t.deleted AS has_deleted_template 
FROM templates t WITH(NOLOCK)
CROSS APPLY (
	SELECT TOP 1 template_name, created_utc, template_id, created_by, resource_path
	FROM reports r WITH(NOLOCK)
	WHERE deleted = 0
		AND r.template_id = t.id
	ORDER BY created_utc
) r
