using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Repo;

namespace MVE.Service.Banner
{
    public class StaticContentBannerService : IStaticContentBannerService
    {
        IRepository<StaticContentBanner> _repoStaticContentBanner;
        public StaticContentBannerService(IRepository<StaticContentBanner> repoStaticContentBanner)
        {
            _repoStaticContentBanner = repoStaticContentBanner;
        }
        public PagedListResult<StaticContentBanner> Get(SearchQuery<StaticContentBanner> query, out int totalItems)
        {
            return _repoStaticContentBanner.Search(query, out totalItems);
        }
        public StaticContentBanner GetStaticContentBannerById(int Id)
        {
            return _repoStaticContentBanner.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public StaticContentBanner GetStaticContentBanner()
        {
            return _repoStaticContentBanner.Query().Filter(x => x.IsActive.Equals(true)).Get().FirstOrDefault();
        }
        public List<StaticContentBanner> GetListStaticContentBannerById(int imgeid)
        {
            return _repoStaticContentBanner.Query().Filter(x =>x.ImageFor==imgeid && x.IsActive==true).Get().ToList();
        }
        public async Task<StaticContentBanner> SaveStaticContentBanner(StaticContentBanner accommodation)
        {
            await _repoStaticContentBanner.InsertAsync(accommodation);
            return accommodation;
        }
        public async Task<StaticContentBanner> UpdateStaticContentBanner(StaticContentBanner accommodation)
        {
            await _repoStaticContentBanner.UpdateAsync(accommodation);
            return accommodation;
        }
       
    }
}
