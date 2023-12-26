using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service.Banner
{
    public interface IStaticContentBannerService
    {

        PagedListResult<StaticContentBanner> Get(SearchQuery<StaticContentBanner> query, out int totalItems);
        StaticContentBanner GetStaticContentBannerById(int Id);
        StaticContentBanner GetStaticContentBanner();
        List<StaticContentBanner> GetListStaticContentBannerById(int imgid);
        Task<StaticContentBanner> UpdateStaticContentBanner(StaticContentBanner accommodation);
        Task<StaticContentBanner> SaveStaticContentBanner(StaticContentBanner accommodation);
    }
}
