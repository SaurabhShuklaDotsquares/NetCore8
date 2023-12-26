using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service.ContentManager
{
    public interface IContentManagerService:IDisposable
    {
        PagedListResult<StaticPage> Get(SearchQuery<StaticPage> query, out int totalItems);
        StaticPage GetstaticpageById(int Id);
        Task<StaticPage> UpdatestaticPage(StaticPage staticPage);
    }
}
