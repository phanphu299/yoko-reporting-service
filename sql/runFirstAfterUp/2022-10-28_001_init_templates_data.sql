IF NOT EXISTS(SELECT 1 FROM templates
              where name = 'Default')
BEGIN
	DECLARE @maxInt INT = 2147483647;
	SET IDENTITY_INSERT templates ON;
    INSERT INTO templates(id, name, template_file_url, output_type_id, storage_id, [default], deleted)
    VALUES (@maxInt,'Default', '', 'EXCELOPENXML', 1, '{"ReportName":"default"}', 1);
	SET IDENTITY_INSERT templates OFF;
    DBCC CHECKIDENT (templates, RESEED, 0);
END