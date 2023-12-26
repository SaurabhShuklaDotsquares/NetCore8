using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ThemeService : IThemeService
    {
        IRepository<Theme> _repoTheme;
        public ThemeService(IRepository<Theme> repoTheme)
        {
            _repoTheme = repoTheme;
        }

        public PagedListResult<Theme> Get(SearchQuery<Theme> query, out int totalItems)
        {
            return _repoTheme.Search(query, out totalItems);
        }
        public Theme GetThemeById(int Id)
        {
            return _repoTheme.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public Theme GetThemeByName(string Name)
        {
            return _repoTheme.Query().Filter(x => x.Name.Equals(Name)).Get().FirstOrDefault();
        }
        public async Task<Theme> SaveTheme(Theme accommodation)
        {
            await _repoTheme.InsertAsync(accommodation);
            return accommodation;
        }
        public async Task<Theme> UpdateTheme(Theme accommodation)
        {
            await _repoTheme.UpdateAsync(accommodation);
            return accommodation;
        }
        public bool IsThemeExists(string name)
        {
            bool isExist = _repoTheme.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }
        public bool IsThemeShortNameExists(string shortname, int ids)
        {
            bool isExist = _repoTheme.Query().Filter(x => x.ShortName.Trim().Replace(" ", "").ToLower().Equals(shortname.Trim().Replace(" ", "").ToLower()) && x.Id != ids && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }
        //public List<SelectListItem> GetThemesForDropDown(bool isActive = true)
        //{
        //    List<SelectListItem> list = new List<SelectListItem>();
        //    var _data = _repoTheme.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive).ToList()?.Select(y => new SelectListItem
        //   // var _data = _repoTheme.Query().AsNoTracking().Get().ToList()?.Select(y => new SelectListItem
        //    {
        //        Text = y.Name,
        //        Value = y.Id.ToString()
        //    });
        //    list.AddRange(_data);
        //    return list;
        //}

        public IEnumerable<Theme> GetAllThemes(bool isActive, bool isDeleted)
        {
            return _repoTheme.Query().AsTracking().Filter(x => (x.IsActive || x.IsActive == isActive) && x.IsDeleted == isDeleted).Get();
        }
    }
}
