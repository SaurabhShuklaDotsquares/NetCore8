using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;

namespace MVE.Core.Code.Notification
{
    public class CreateNotification
    {
        public UserNotification CreateUserNotification(long userId, int userIdType, int notificationTypeId, long CreatedBy, int CreatedByType, string? Subject, string Message)
        {
            var userNotification = new UserNotification
            {
                UserId = userId,
                UserIdType = userIdType,
                ImageName = "",
                Title = (Subject!= null && Subject != "") ? Subject : "Title Not Available",
                Descriptions = Message,
                NotificationTypeId = notificationTypeId,
                SentType = 1,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = CreatedBy,
                CreatedByType = CreatedByType,
                ImageExtension = ".PNG",
                Subject = (Subject != null && Subject != "") ? Subject : "Title Not Available",
            };
            return userNotification;
        }
    }
}
