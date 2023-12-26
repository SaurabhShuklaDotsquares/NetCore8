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
    public class NotificationService : INotificationService
    {
        IRepository<UserNotification> _repoUserNotification;
        IRepository<NotificationType> _repoUserNotificationType;
        public NotificationService(IRepository<UserNotification> repoUserNotification, IRepository<NotificationType> repoUserNotificationType)
        {
            _repoUserNotification = repoUserNotification;
            _repoUserNotificationType= repoUserNotificationType;
        }
        public List<UserNotification> GetAllUserNotification()
        {
            return _repoUserNotification.Query().Get().ToList();
        }

        public List<UserNotification> GetAllUnvisitedUserNotification()
        { 
            return _repoUserNotification.Query().Filter(x=>x.IsVisited == false).Get().ToList();
        }

        public async Task<UserNotification> SaveUserNotification(UserNotification obj)
        {
            try
            {
                await _repoUserNotification.InsertAsync(obj);
                return obj;
            }
            catch(Exception ex) 
            {
                return null;
            }
        }
        public async Task<UserNotification> UpdateUserNotification(UserNotification obj)
        {
            await _repoUserNotification.UpdateAsync(obj);
            return obj;
        }
        public List<NotificationType> GetAllNotificationType()
        {
            return _repoUserNotificationType.Query().Get().ToList();
        }
        public UserNotification GetUserNotificationById(int Id)
        {
            return _repoUserNotification.Query().Filter(x => x.Id.Equals(Id)).Get().FirstOrDefault();
        }
        public PagedListResult<UserNotification> Get(SearchQuery<UserNotification> query, out int totalItems)
        {
            return _repoUserNotification.Search(query, out totalItems);
        }

        public List<UserNotification> GetNotificationByUserId(int Id)
        {
            return _repoUserNotification.Query().Filter(x => x.UserId==Id).Get().ToList();
        }

    }
}
