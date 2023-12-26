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
    public interface IUserRoleService
    {
        PagedListResult<UserRole> Get(SearchQuery<UserRole> query, out int totalItems);
        UserRole GetUserRoleById(long Id);
        Task<UserRole> SaveUserRole(UserRole role);
        Task<UserRole> UpdateUserRole(UserRole role);
        bool IsRoleExists(string name);
        List<SelectListItem> GetUserRolesForDropDown(bool isActive = true);
        IEnumerable<RolePage> GetALLRolePages();
        IEnumerable<RolePagePermission> GetRolePagePermissionsByRoleId(long id);
        Task<bool> DeletePermission(List<RolePagePermission> rolePagePermissions);
        Task InsertPermissionList(List<RolePagePermission> rolePagePermissions);
        int[] GetPermissionByRoleId(long roleId);
        List<SelectListItem> GetUserRolesForDropDownNotSuperAdmin();
    }
}
