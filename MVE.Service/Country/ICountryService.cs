using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface ICountryService
    {
        PagedListResult<CountryMaster> Get(SearchQuery<CountryMaster> query, out int totalItems);
        CountryMaster GetCountryMasterById(int Id);
        Task<CountryMaster> SaveCountryMaster(CountryMaster accommodation);
        Task<CountryMaster> UpdateCountryMaster(CountryMaster accommodation);
        bool IsCountryMasterExists(string name);
        List<SelectListItem> GetCountryMastersForDropDown(bool isActive = true);
        IEnumerable<CountryMaster> GetAllCountries(bool isActive, bool isDeleted);

        List<SelectListItem> GetCountryMastersForDropDownWithShortURL(bool isActive = true);

        bool IsCountryShortNameExists(string shortname, int ids);
        string GetCountryNamebyShortName(string shortname);
        CountryMaster GetCountrybyShortName(string shortname);
        StateMaster GetStateMasterById(int Id);

        List<CountryMaster> GetAllCountriesWithoutStateMatch(bool isActive, bool isDeleted);
        CountryMaster GetCountryMasterByName(string name);
    }
}
