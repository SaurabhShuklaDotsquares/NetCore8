using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;
using MVE.Admin.Models;

namespace MVE.Service
{
    public interface ICategoryService
    {
        PagedListResult<Categories> Get(SearchQuery<Categories> query, out int totalItems);
        Categories GetCategoryById(int Id);
        Task<Categories> SaveCategory(Categories accommodation);
        Task<Categories> UpdateCategory(Categories accommodation);
        bool IsCategoryExists(string name);
        List<SelectListItem> GetParentCategoryForDropDown(bool isActive = true);
        List<SelectListItem> GetChildCategoryForDropDown(bool isActive = true);
        IEnumerable<Categories> GetAllCategory(bool isActive, bool isDeleted);
        Categories GetCategoryByName(string name);
        int CountChildCategory(int Id);
    }
}
