using System;
using System.Collections.Generic;
using System.Linq;

namespace PerplexDashboards.Models.UserDashboard
{   
    public class Paging
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalResults * 1.0 / PageSize);
        public IList<int> Pagination { get; set; }

        public Paging(int page, int pageSize, int totalResults)
        {            
            Page = page;
            PageSize = pageSize;
            TotalResults = totalResults;
            Pagination = GetPagination(2);
        }

        public IList<int> GetPagination(int n)
        {
            var pageNumbers = new List<int>();

            // Always show first page
            pageNumbers.Add(1);

            // After the first page, never show a page < 2
            int start = Math.Max(2, Page - n);

            // Het kan zijn dat we tever aan het eind zitten waardoor
            // de pagina's na de huidige pagina voorbij het totaal zitten.
            // In dat geval meer van de pagina's voor de huidige pagina laten zien,
            // zodat het totaal aantal pagina's indien mogelijk 3 + 2*n blijft.
            if (Page + n >= TotalPages)
            {
                start = Math.Max(2, start - (1 + Page + n - TotalPages));
            }

            for (int page = start; page <= Math.Min(start + 2 * n, TotalPages - 1); page++)
            {
                pageNumbers.Add(page);
            }

            pageNumbers.Add(TotalPages);

            return pageNumbers;
        }
    }
}