using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Repo;

namespace MVE.Service
{
    public class StaticService : IStaticService
    {
        private readonly IRepository<StaticPage> repoStaticPage;
        public StaticService(IRepository<StaticPage> repoStaticPage)
        {
            this.repoStaticPage = repoStaticPage;
        }

        public PagedListResult<StaticPage> GetStaticPage(SearchQuery<StaticPage> query, out int totalItems)
        {
            return repoStaticPage.Search(query, out totalItems);
        }
        public StaticPage GetStaticPageByPageId(int id)
        {
            return repoStaticPage.FindById(id);
        }
        public StaticPage GetStaticPageByUrl(string url)
        {
            return repoStaticPage.Query().Filter(x => x.Url == url&&x.IsActive).Get().FirstOrDefault();
        }

        public List<StaticPage> GetActiveStaticPage()
        {
            return repoStaticPage.Query().Filter(c => c.IsActive).Get().ToList();
        }

        public List<StaticPage> SearchStaticPage(string keyword, int skip, int take = 10)
        {
            var staticPageQuery = repoStaticPage.Query().Filter(c =>
                        !string.IsNullOrEmpty(keyword) &&
                        c.Name.ToLower().Contains(keyword.ToLower()) ||
                        c.MetaDescription.ToLower().Contains(keyword.ToLower()))
                    .GetQuerable()
                    .OrderByDescending(x => x.AddedDate);

            if (take > 0)
            {
                return staticPageQuery.Skip(skip).Take(take).ToList();
            }

            return staticPageQuery.ToList();
        }

        public StaticPage Save(StaticPage staticPage)
        {
            staticPage.AddedDate = DateTime.UtcNow;
            repoStaticPage.Insert(staticPage);
            return staticPage;
        }
        public StaticPage Update(StaticPage staticPage)
        {
            staticPage.ModifyDate = DateTime.UtcNow;
            repoStaticPage.Update(staticPage);
            return staticPage;
        }
    }
}
