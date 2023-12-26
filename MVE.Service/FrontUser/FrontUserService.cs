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
    public class FrontUserService : IFrontUserService
    {
        IRepository<User> _repoFrontUser;
        public FrontUserService(IRepository<User> repoFrontUser)
        {
            _repoFrontUser = repoFrontUser;
        }

        public User GetFrontUserByEmail(string email)
        {
            return _repoFrontUser.Query().Filter(x => x.Email.Equals(email) && !x.IsDeleted).Get().FirstOrDefault();
        }
        public User GetFrontUserById(long Id)
        {
            return _repoFrontUser.Query().Filter(x => x.Id.Equals(Id) && !x.IsDeleted).Get().FirstOrDefault();
        }
        public User GetFrontUserByGuid(string guid)
        {
            return _repoFrontUser.Query().Filter(x => x.ForgotPasswordLink.Equals(guid) && !x.IsDeleted).Get().FirstOrDefault();
        }
        //public User UpdateFrontUser(User user)
        //{
        //    _repoFrontUser.Update(user);
        //    return user;
        //}
        public async Task<User> UpdateFrontUser(User user)
        {
            await _repoFrontUser.UpdateAsync(user);
            return user;
        }
        public async Task<User> UpdateFrontUserAsync(User frontUser)
        {
            await _repoFrontUser.UpdateAsync(frontUser);
            return frontUser;
        }
        public async Task<User> SaveFrontUser(User user)
        {
            await _repoFrontUser.InsertAsync(user);
            return user;
        }
        public PagedListResult<User> Search(SearchQuery<User> query, out int totalCount)
        {
            return _repoFrontUser.Search(query, out totalCount);
        }
        public List<User> GetAllUser()
        {
            return _repoFrontUser.Query().Filter(x=>x.IsDeleted==false).Get().ToList();
        }
    }
}
