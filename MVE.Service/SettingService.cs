using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Code.Extensions;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Repo;

namespace MVE.Service
{
    public class SettingService : ISettingService
    {
        IRepository<GeneralSiteSetting> _repoSetting;
        public SettingService(IRepository<GeneralSiteSetting> repoSetting)
        {
            _repoSetting = repoSetting;
        }

      
        public List<GeneralSiteSetting> GetGeneralSiteSettings()
        {
            return _repoSetting.Query().Filter(x=>x.IsActive).Get().ToList();
        }
        public GeneralSiteSetting GetGeneralSiteSettingsByKey(string keyname)
        {
            return _repoSetting.Query().Filter(x=>x.IsActive&&x.KeyName==keyname).Get().FirstOrDefault();
        }
        public async Task<GeneralSiteSetting> SaveGeneralSiteSettings(GeneralSiteSetting generalSiteSetting)
        {
            await _repoSetting.InsertAsync(generalSiteSetting);
            return generalSiteSetting;
        }
        public async Task<GeneralSiteSetting> UpdateGeneralSiteSettings(GeneralSiteSetting generalSiteSetting)
        {
            await _repoSetting.UpdateAsync(generalSiteSetting);
            return generalSiteSetting;
        }
    }
}
