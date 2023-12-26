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
    public class CountryService : ICountryService
    {
        IRepository<CountryMaster> _repoCountryMaster;
        IRepository<StateMaster> _repoStateMaster;
        public CountryService(IRepository<CountryMaster> repoCountryMaster, IRepository<StateMaster> repoStateMaster)
        {
            _repoCountryMaster = repoCountryMaster;
            _repoStateMaster = repoStateMaster;
        }

        public PagedListResult<CountryMaster> Get(SearchQuery<CountryMaster> query, out int totalItems)
        {
            return _repoCountryMaster.Search(query, out totalItems);
        }
        public CountryMaster GetCountryMasterById(int Id)
        {
            return _repoCountryMaster.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public CountryMaster GetCountryMasterByName(string name)
        {
            return _repoCountryMaster.Query().Filter(x => x.Name.Equals(name)).Get().FirstOrDefault();
        }
        public async Task<CountryMaster> SaveCountryMaster(CountryMaster accommodation)
        {
            await _repoCountryMaster.InsertAsync(accommodation);
            return accommodation;
        }
        public async Task<CountryMaster> UpdateCountryMaster(CountryMaster accommodation)
        {
            await _repoCountryMaster.UpdateAsync(accommodation);
            return accommodation;
        }
        public bool IsCountryMasterExists(string name)
        {
            bool isExist = _repoCountryMaster.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;

            return isExist;
        }
        public List<SelectListItem> GetCountryMastersForDropDown(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoCountryMaster.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive).ToList()?.Select(y => new SelectListItem

            {
                Text = y.Name,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            return list;
        }
        public IEnumerable<CountryMaster> GetAllCountries(bool isActive, bool isDeleted)
        {
            //var data = _repoCountryMaster.Query().AsTracking().Filter(x => (x.IsActive || x.IsActive == isActive && x.IsDeleted == isDeleted)).Get();
            //data = data.ToList().Where(x => x.StateMaster != null).Distinct();

            //var data = _repoStateMaster.Query().AsTracking().Get();
            //var country = data.ToList().Where(x => x.IdNavigation != null).Select(x => x.IdNavigation).Distinct().ToList();

            var State = _repoStateMaster.Query().AsNoTracking().Get().ToList();
            var Country = _repoCountryMaster.Query().AsNoTracking().Get().Where(x => x.IsActive == isActive && x.IsDeleted == isDeleted).ToList();

            var countries = (from state in State
                             join country in Country on state.CountryId equals country.Id
                             select country).Distinct().ToList();
            return countries;
        }

        public List<SelectListItem> GetCountryMastersForDropDownWithShortURL(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoCountryMaster.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive).ToList()?.Select(y => new SelectListItem

            {
                Text = y.Name,
                Value = y.ShortUrl.ToString()
            });
            list.AddRange(_data);
            return list;
        }

        public bool IsCountryShortNameExists(string shortname, int ids)
        {
            bool isExist = _repoCountryMaster.Query().Filter(x => x.ShortUrl.Trim().Replace(" ", "").ToLower().Equals(shortname.Trim().Replace(" ", "").ToLower()) && x.Id != ids && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }

        public string GetCountryNamebyShortName(string shortname)
        {
            return _repoCountryMaster.Query().Filter(x => x.ShortUrl.Trim().Replace(" ", "").ToLower().Equals(shortname.Trim().Replace(" ", "").ToLower())).Get().FirstOrDefault()?.Name ?? "NA";

        }
        public CountryMaster GetCountrybyShortName(string shortname)
        {
            return _repoCountryMaster.Query().Filter(x => x.ShortUrl.Trim().Replace(" ", "").ToLower().Equals(shortname.Trim().Replace(" ", "").ToLower())).Get().FirstOrDefault();

        }
        public StateMaster GetStateMasterById(int Id)
        {
            return _repoStateMaster.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public List<CountryMaster> GetAllCountriesWithoutStateMatch(bool isActive, bool isDeleted)
        {
            var countries = _repoCountryMaster.Query().AsNoTracking().Get().Where(x => x.IsActive == isActive && x.IsDeleted == isDeleted).ToList();

            return countries;
        }
    }
}
