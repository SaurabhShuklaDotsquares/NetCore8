using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;

namespace MVE.Service
{
    public interface ISettingService
    {
        List<GeneralSiteSetting> GetGeneralSiteSettings();
        GeneralSiteSetting GetGeneralSiteSettingsByKey(string keyname);
        Task<GeneralSiteSetting> SaveGeneralSiteSettings(GeneralSiteSetting generalSiteSetting);
        Task<GeneralSiteSetting> UpdateGeneralSiteSettings(GeneralSiteSetting generalSiteSetting);
    }
}
