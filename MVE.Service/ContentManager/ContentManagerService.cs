using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Repo;

namespace MVE.Service.ContentManager
{
    public class ContentManagerService : IContentManagerService
    {

        IRepository<StaticPage> _repoStaticPage;
        public ContentManagerService(IRepository<StaticPage> repoStaticPage)
        {
            _repoStaticPage = repoStaticPage;
        }
        public PagedListResult<StaticPage> Get(SearchQuery<StaticPage> query, out int totalItems)
        {
            return _repoStaticPage.Search(query, out totalItems);
        }
        public StaticPage GetstaticpageById(int Id)
        {
            return _repoStaticPage.Query().Filter(x => x.StaticPageId.Equals(Id)).Get().FirstOrDefault();
        }
        public async Task<StaticPage> UpdatestaticPage(StaticPage staticPage)
        {
            await _repoStaticPage.UpdateAsync(staticPage);
            return staticPage;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
