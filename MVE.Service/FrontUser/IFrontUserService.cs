using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface IFrontUserService
    {
        User GetFrontUserByEmail(string email);
        User GetFrontUserById(long Id);
        User GetFrontUserByGuid(string guid);
        Task<User> SaveFrontUser(User user);
        Task<User> UpdateFrontUser(User user);
        Task<User> UpdateFrontUserAsync(User frontUser);
        PagedListResult<User> Search(SearchQuery<User> query, out int totalCount);
        List<User> GetAllUser();
    }
}