using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.Repo;

namespace MVE.Service
{
    public class RegionService : IRegionService
    {
        IRepository<Region> _repoRegion;
        public RegionService(IRepository<Region> repoRegion)
        {
            _repoRegion = repoRegion;
        }
        public IEnumerable<Region> GetAllRegions(bool isActive, bool isDeleted)
        {
            return _repoRegion.Query().AsTracking().Filter(x => (x.IsActive || Convert.ToBoolean(x.IsActive) == Convert.ToBoolean(isActive)) && x.IsDeleted == isDeleted).Get();
        }
    }
}
