exec sp_rename 'dbo.reports.file_url', 'storage_url', 'COLUMN';
go

alter table reports
add file_name nvarchar(255) not null default '';

alter table reports
add download_url varchar(100) not null default '';