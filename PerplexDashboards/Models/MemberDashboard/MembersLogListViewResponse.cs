namespace PerplexDashboards.Models.MemberDashboard
{
    public class MembersLogListViewResponse : BaseListviewResponse
    {
        public string DateTime { get; set; }
        public string Action { get; set; }
        public string IPAddress { get; set; }
    }
}