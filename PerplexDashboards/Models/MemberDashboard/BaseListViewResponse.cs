namespace PerplexDashboards.Models.MemberDashboard
{
    public abstract class BaseListviewResponse
    {
        // id / icon / name / published zijn lowercase omdat dit standaard zo is is voor Umbraco ListView items,
        // deze worden dus ook in de View zo gebruikt. Onze custom properties doen we wel gewoon Capitalized.
        public int id { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public bool published { get; set; }
    }
}