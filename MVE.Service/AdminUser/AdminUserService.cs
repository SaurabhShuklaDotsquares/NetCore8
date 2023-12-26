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
    public class AdminUserService : IAdminUserService
    {
        IRepository<AdminUser> _repoAdminUser;
        public AdminUserService(IRepository<AdminUser> repoAdminUser)
        {
            _repoAdminUser = repoAdminUser;
        }

        public AdminUser GetAdminUserByEmail(string email)
        {
            return _repoAdminUser.Query().Filter(x => x.Email.Equals(email) && !x.IsDeleted).Include(x => x.Role).Get().FirstOrDefault();
        }
        public AdminUser GetAdminUserById(long Id)
        {
            return _repoAdminUser.Query().Filter(x => x.Id.Equals(Id) && !x.IsDeleted).Include(x => x.Role).Get().FirstOrDefault();
        }
        public AdminUser GetAdminUserByGuid(string guid)
        {
            return _repoAdminUser.Query().Filter(x => x.ForgotPasswordLink.Equals(guid) && !x.IsDeleted).Include(x => x.Role).Get().FirstOrDefault();
        }
        public AdminUser UpdateAdminUser(AdminUser user)
        {
            _repoAdminUser.Update(user);
            return user;
        }
        public AdminUser SaveAdminUser(AdminUser user)
        {
            _repoAdminUser.Insert(user);
            return user;
        }
        public async Task<AdminUser> UpdateAdminUserAsync(AdminUser adminUser)
        {
            await _repoAdminUser.UpdateAsync(adminUser);
            return adminUser;
        }
        public bool IsRoleInUse(long Id)
        {
            IEnumerable<AdminUser> users = _repoAdminUser.Query().Filter(x => x.RoleId == Id && (x.IsActive == true) && !x.IsDeleted).Get();
            if (users.Count() > 0)
                return true;
            else
                return false;
        }
        public PagedListResult<AdminUser> Search(SearchQuery<AdminUser> query, out int totalCount)
        {
            return _repoAdminUser.Search(query, out totalCount);
        }
        public bool IsAdminUserExists(string email)
        {
            bool isExist = _repoAdminUser.Query().Filter(x => x.Email.Trim().Replace(" ", "").ToLower().Equals(email.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;
            return isExist;
        }
        public async Task<AdminUser> SaveAdminUserAsync(AdminUser adminUser)
        {
            await _repoAdminUser.UpdateAsync(adminUser);
            return adminUser;
        }
    }
}
