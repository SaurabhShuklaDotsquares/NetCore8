using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Data.Models;
using MVE.Dto;

namespace MVE.Service
{
    public interface IEmailFactoryService
    {
        Task<bool> SendAdminUserForgotPasswordEmail(AdminUser user, string email);
        Task<bool> SendUserForgotPasswordEmail(User user, string email);
        Task<bool> SendOTPEmail(User user);
        Task<bool> SendCustomPlanEmail(User user, long id, char sendFrom,string generatedPwd, bool isNewUser);
        Task<bool> SendVerificationEmail(User user, char sendFrom, string password, bool isNewUser);
        Task<bool> SendQuoteEmail(long QuoteFeedbackId, long QuoteForEnqueryId, long UserId, long PackageId, string QuoteVerificationToken, string UserName, string Email, int FeedbackForEnqueryType, string PackageUrlForQuote, string Subject, string EmailType, string EmailCustomContent,string TestField);
        Task<bool> SendContactQueryReply(UserNotification notification, string contactEmail, string contactUser);
        Task<bool> SendEmailBookingSuccess(BookingSuccessDto model);
        Task<bool> SendFrontUserEmail(string msg, string email,string subject);//Arvind Kumar 28-11-2023
        Task<bool> SendNotification(UserNotification userNotification,string userName,string email);
    }
}
