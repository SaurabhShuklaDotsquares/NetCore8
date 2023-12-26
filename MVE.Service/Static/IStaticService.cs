using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface IStaticService
    {
        PagedListResult<StaticPage> GetStaticPage(SearchQuery<StaticPage> query, out int totalItems);
        List<StaticPage> GetActiveStaticPage();
        List<StaticPage> SearchStaticPage(string keyword, int skip, int take = 10);

        StaticPage GetStaticPageByPageId(int id);

        StaticPage Save(StaticPage staticPage);
        StaticPage Update(StaticPage staticPage);
        StaticPage GetStaticPageByUrl(string url);
    }
}
