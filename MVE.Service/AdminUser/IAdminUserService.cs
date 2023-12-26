using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface IAdminUserService
    {
        AdminUser GetAdminUserByEmail(string email);
        AdminUser GetAdminUserById(long Id);
        AdminUser GetAdminUserByGuid(string guid);
        AdminUser UpdateAdminUser(AdminUser user);
        Task<AdminUser> UpdateAdminUserAsync(AdminUser adminUser);
        bool IsRoleInUse(long RoleId);
        PagedListResult<AdminUser> Search(SearchQuery<AdminUser> query, out int totalCount);
        AdminUser SaveAdminUser(AdminUser user);
        bool IsAdminUserExists(string email);
        Task<AdminUser> SaveAdminUserAsync(AdminUser adminUser);
    }
}
