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
using MVE.Admin.Models;

namespace MVE.Service
{
    public class CategoryService : ICategoryService
    {
        IRepository<Categories> _repoCategorie;
        public CategoryService(IRepository<Categories> repoCategorie)
        {
            _repoCategorie = repoCategorie;
        }

        public PagedListResult<Categories> Get(SearchQuery<Categories> query, out int totalItems)
        {
            return _repoCategorie.Search(query, out totalItems);
        }
        public Categories GetCategoryById(int Id)
        {
            return _repoCategorie.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public Categories GetCategoryByName(string name)
        {
            return _repoCategorie.Query().Filter(x => x.Name.Equals(name)).Get().FirstOrDefault();
        }
        public async Task<Categories> SaveCategory(Categories accommodation)
        {
            await _repoCategorie.InsertAsync(accommodation);
            return accommodation;
        }
        public async Task<Categories> UpdateCategory(Categories accommodation)
        {
            await _repoCategorie.UpdateAsync(accommodation);
            return accommodation;
        }
        public bool IsCategoryExists(string name)
        {
            bool isExist = _repoCategorie.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;

            return isExist;
        }
        public List<SelectListItem> GetParentCategoryForDropDown(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoCategorie.Query().AsNoTracking().Get().Where(x => x.IsDeleted==false && x.IsActive==true && x.ParentId==0).ToList()?.Select(y => new SelectListItem

            {
                Text = y.Name,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            list.Insert(0, new SelectListItem() { Value = "0", Text = "-Select-" });
            return list;
        }
        public List<SelectListItem> GetChildCategoryForDropDown(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoCategorie.Query().AsNoTracking().Get().Where(x => x.IsDeleted == false && x.IsActive == true && x.ParentId>0).ToList()?.Select(y => new SelectListItem

            {
                Text = y.Name,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            
            return list;
        }
        public IEnumerable<Categories> GetAllCategory(bool isActive, bool isDeleted)
        {
            var Categorie = _repoCategorie.Query().AsNoTracking().Get().Where(x => x.IsActive == isActive && x.IsDeleted == isDeleted).ToList();
            return Categorie;
        }

        

        public bool IsCategoryNameExists(string shortname, int ids)
        {
            bool isExist = _repoCategorie.Query().Filter(x => x.Name.Trim().Replace(" ", "").ToLower().Equals(shortname.Trim().Replace(" ", "").ToLower()) && x.Id != ids && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }

        public int CountChildCategory(int id)
        {
            var _data = _repoCategorie.Query().AsNoTracking().Get().Where(x => x.IsDeleted == false && x.IsActive == true && x.ParentId == id).ToList();
            return _data.Count;
        }
    }
}
