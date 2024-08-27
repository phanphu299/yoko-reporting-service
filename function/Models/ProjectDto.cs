namespace Reporting.Function.Model
{
    public class ProjectDto
    {
        public const string ASSET_DASHBOARD_TYPE = "asset_dashboard";

        public string Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public bool IsMigrated { get; set; }
        public string ProjectType { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionResourceId { get; set; }
        public string SubscriptionName { get; set; }
    }
}