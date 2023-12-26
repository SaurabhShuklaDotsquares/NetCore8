using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.DataTable.Search;

namespace MVE.Service
{
    public interface INotificationService
    {
        List<UserNotification> GetAllUserNotification();
        List<UserNotification> GetAllUnvisitedUserNotification();
        Task<UserNotification> SaveUserNotification(UserNotification obj);
        Task<UserNotification> UpdateUserNotification(UserNotification obj);
        List<NotificationType> GetAllNotificationType();
        UserNotification GetUserNotificationById(int Id);
        PagedListResult<UserNotification> Get(SearchQuery<UserNotification> query, out int totalItems);

        List<UserNotification> GetNotificationByUserId(int Id);
    }
}
