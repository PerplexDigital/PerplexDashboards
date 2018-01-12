namespace PerplexDashboards.Models.MemberDashboard
{
    public class LockedMembersListViewResponse : BaseListviewResponse
    {
        public string Email { get; set; }
        public string LastLoginDate { get; set; }
    }
}