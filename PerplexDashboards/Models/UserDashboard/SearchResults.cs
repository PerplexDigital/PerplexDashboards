using System;
using System.Collections.Generic;
using System.Linq;

namespace PerplexDashboards.Models.UserDashboard
{
    /// <summary>
    /// De filters die in Umbraco kunnen worden ingesteld en naar de API controller worden gestuurd
    /// om zoekresultaten op te leveren
    /// </summary>
    public class SearchResults<T> where T : class
    {
        public IList<T> Items { get; set; }
        public Paging Paging { get; set; }

        public SearchResults(IEnumerable<T> items, int totalResults, int page, int pageSize)
        {
            Items = items.ToList();
            Paging = new Paging(page, pageSize, totalResults);            
        }
    }
}