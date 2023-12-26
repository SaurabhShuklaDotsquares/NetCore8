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
    public class UserRoleService : IUserRoleService
    {
        IRepository<UserRole> _repoUserRole;
        IRepository<RolePage> _repoRolePage;
        IRepository<RolePagePermission> _repoRolePagePermission;
        IRepository<MenuItem> _repoMenuItem;
        public UserRoleService(IRepository<UserRole> repoUserRole, IRepository<RolePage> repoRolePage, IRepository<RolePagePermission> repoRolePagePermission, IRepository<MenuItem> repoMenuItem)
        {
            _repoUserRole = repoUserRole;
            _repoRolePage = repoRolePage;
            _repoRolePagePermission = repoRolePagePermission;
            _repoMenuItem = repoMenuItem;
        }

        public PagedListResult<UserRole> Get(SearchQuery<UserRole> query, out int totalItems)
        {
            return _repoUserRole.Search(query, out totalItems);
        }
        public UserRole GetUserRoleById(long Id)
        {
            return _repoUserRole.Query().Filter(x => x.Id.Equals(Id)).Include(x => x.UserPermissions).Get().FirstOrDefault();
            //return _repoUserRole.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public async Task<UserRole> SaveUserRole(UserRole role)
        {
            await _repoUserRole.InsertAsync(role);
            return role;
        }
        public async Task<UserRole> UpdateUserRole(UserRole role)
        {
            await _repoUserRole.UpdateAsync(role);
            return role;
        }
        public bool IsRoleExists(string name)
        {
            bool isExist = _repoUserRole.Query().Filter(x => x.RoleName.Trim().Replace(" ", "").ToLower().Equals(name.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }
        public List<SelectListItem> GetUserRolesForDropDown(bool isActive = true)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoUserRole.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive).ToList()?.Select(y => new SelectListItem
            {
                Text = y.RoleName,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            return list;
        }
        public IEnumerable<RolePage> GetALLRolePages()
        {
            return _repoRolePage.Query().AsTracking().Filter(x => x.IsActive)
                .Include(x => x.RolePagePermissions)
                .Get();
        }
        public IEnumerable<RolePagePermission> GetRolePagePermissionsByRoleId(long id)
        {
            return _repoRolePagePermission.Query().AsTracking().Filter(x => x.RoleId == id)
                .Get();
        }
        public async Task<bool> DeletePermission(List<RolePagePermission> rolePagePermissions)
        {
            await _repoRolePage.DeleteRange(rolePagePermissions);
            return true;
        }
        public async Task InsertPermissionList(List<RolePagePermission> rolePagePermissions)
        {
            foreach (var per in rolePagePermissions)
            {
                _repoRolePagePermission.InsertGraph(per);
            }
        }
        public int[] GetPermissionByRoleId(long roleId)
        {
            return _repoRolePagePermission.Query().Filter(x => x.RoleId.Equals(roleId) && (x.IsReadOnly || x.IsCreate || x.IsEdit || x.IsDelete)).Get().Select(x => Convert.ToInt32(x.PageId)).ToArray();
        }
        public List<SelectListItem> GetUserRolesForDropDownNotSuperAdmin()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var _data = _repoUserRole.Query().AsNoTracking().Get().Where(x => !x.IsDeleted && x.IsActive && x.Id != 1).ToList()?.Select(y => new SelectListItem
            {
                Text = y.RoleName,
                Value = y.Id.ToString()
            });
            list.AddRange(_data);
            return list;
        }
    }
}
