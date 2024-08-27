if not exists(select 1 from output_types where id = 'PDF')
begin
    insert into output_types(id, name, extension)
    values ('PDF', 'PDF (.pdf)', '.pdf')
end
else
begin
    update output_types set name = 'PDF (.pdf)', extension = '.pdf'
    where id = 'PDF'
end

if not exists(select 1 from output_types where id = 'EXCELOPENXML')
begin
    insert into output_types(id, name, extension)
    values ('EXCELOPENXML', 'Excel (.xlsx)', '.xlsx')
end
else
begin
    update output_types set name = 'Excel (.xlsx)', extension = '.xlsx'
    where id = 'EXCELOPENXML'
end

if not exists(select 1 from output_types where id = 'WORDOPENXML')
begin
    insert into output_types(id, name, extension)
    values ('WORDOPENXML', 'Word (.docx)', '.docx')
end
else
begin
    update output_types set name = 'Word (.docx)', extension = '.docx'
    where id = 'WORDOPENXML'
end