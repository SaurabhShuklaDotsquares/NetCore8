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
    public interface IUserService
    {
        PagedListResult<User> Get(SearchQuery<User> query, out int totalItems);
        User GetUserById(int Id);
        List<User> GetAllUser();
        Billing GetBillingByUserId(long Id);
        Billing GetBillingById(long billingId);
        User GetUserByGuid(string guid);
        Task<User> SaveUser(User userobj);
        Task<User> UpdateUser(User userobj);
        bool IsUserExists(string email);
        User GetUserByEmail(string email);
        Task<Billing> SaveBilling(Billing billing);
        Task<Billing> UpdateBilling(Billing billing);
        List<User> GetAllUserNotDeleted();
        User GetUserById(long Id);
    }
}
