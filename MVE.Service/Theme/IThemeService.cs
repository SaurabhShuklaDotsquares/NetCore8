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
    public interface IThemeService
    {
        PagedListResult<Theme> Get(SearchQuery<Theme> query, out int totalItems);
        Theme GetThemeById(int Id);
        Task<Theme> SaveTheme(Theme themeobj);
        Task<Theme> UpdateTheme(Theme themeobj);
        bool IsThemeExists(string name);
        bool IsThemeShortNameExists(string shortname, int ids);
        //  List<SelectListItem> GetThemesForDropDown(bool isActive = true);
        IEnumerable<Theme> GetAllThemes(bool isActive, bool isDeleted);
        Theme GetThemeByName(string Name);
    }
}
