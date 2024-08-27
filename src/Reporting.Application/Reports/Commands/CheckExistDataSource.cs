namespace Reporting.Application.Command
{
    public class CheckExistDataSource
    {
        public CheckExistDataSource(string currentDataSourceContent)
        {
            CurrentDataSourceContent = currentDataSourceContent;
        }

        public string CurrentDataSourceContent { get; set; }
    }
}