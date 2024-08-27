namespace Reporting.Application.Command.Model
{
    public class UploadStorageSpaceDto
    {
        public string FileUrl { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }

        public UploadStorageSpaceDto(string fileUrl, string folderName, string fileName)
        {
            FileUrl = fileUrl;
            FolderName = folderName;
            FileName = fileName;
        }
    }
}