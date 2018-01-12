namespace PerplexDashboards.Models.MemberDashboard
{
    public class ListViewRequest
    {
        public int? Id { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Filter { get; set; }
        public string OrderDirection { get; set; }
        public string OrderBy { get; set; }
    }
}