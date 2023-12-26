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
    public class UserService : IUserService
    {
        IRepository<User> _repoUser;
        IRepository<Billing> _repobilling;
        public UserService(IRepository<User> repoUser, IRepository<Billing> repobilling)
        {
            _repoUser = repoUser;
            _repobilling = repobilling;
        }

        public PagedListResult<User> Get(SearchQuery<User> query, out int totalItems)
        {
            return _repoUser.Search(query, out totalItems);
        }
        public User GetUserById(int Id)
        {
            return _repoUser.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public List<User> GetAllUser()
        {
            return _repoUser.Query().Get().ToList();
        }

        public User GetUserByGuid(string guid)
        {
            return _repoUser.Query().Filter(x => (x.ForgotPasswordLink ?? "").Equals(guid) && !x.IsDeleted).Get().FirstOrDefault();
        }
        public async Task<User> SaveUser(User userobj)
        {
            await _repoUser.InsertAsync(userobj);
            return userobj;
        }
        public async Task<User> UpdateUser(User userobj)
        {
            await _repoUser.UpdateAsync(userobj);
            return userobj;
        }
        public User GetUserByEmail(string email)
        {
            return _repoUser.Query().Filter(x => x.Email.Equals(email) && !x.IsDeleted).Get().FirstOrDefault();
        }
        public bool IsUserExists(string email)
        {
            bool isExist = _repoUser.Query().Filter(x => x.Email.Trim().Replace(" ", "").ToLower().Equals(email.Trim().Replace(" ", "").ToLower()) && !x.IsDeleted).Get().FirstOrDefault() != null;

            return isExist;
        }
        //<----------------------- user billing--------------------------->
        public Billing GetBillingByUserId(long Id)
        {
            return _repobilling.Query().Filter(x => x.UserId.Equals(Id) && x.IsActive).Get().FirstOrDefault();
        }
        public Billing GetBillingById(long Id)
        {
            return _repobilling.Query().Filter(x => x.Id.Equals(Id) && x.IsActive).Get().FirstOrDefault();
        }
        public async Task<Billing> SaveBilling(Billing billing)
        {
            await _repobilling.InsertAsync(billing);
            return billing;
        }
        public async Task<Billing> UpdateBilling(Billing billing)
        {
            await _repobilling.UpdateAsync(billing);
            return billing;
        }
        public List<User> GetAllUserNotDeleted()
        {
            return _repoUser.Query().Filter(x => x.IsDeleted == false).Get().ToList();
        }
        public User GetUserById(long Id)
        {
            return _repoUser.Query().Filter(x => x.Id.Equals(Id) && !x.IsDeleted).Get().FirstOrDefault();
        }
    }
}
