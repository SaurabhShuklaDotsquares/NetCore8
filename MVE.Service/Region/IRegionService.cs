using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;

namespace MVE.Service
{
    public interface IRegionService
    {
        IEnumerable<Region> GetAllRegions(bool isActive, bool isDeleted);
    }
}
