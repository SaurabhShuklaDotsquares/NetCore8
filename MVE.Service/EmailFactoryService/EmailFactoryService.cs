using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Others;
using MVE.Data.Models;
using MVE.Dto;
namespace MVE.Service
{
    public class EmailFactoryService : IEmailFactoryService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSenderService _emailSenderService;

        private readonly ISettingService _settingService;

        private readonly string LogoImageNameDark;
        public EmailFactoryService(IConfiguration configuration, IEmailSenderService emailSenderService, ISettingService settingService)
        {
            _configuration = configuration;
            _emailSenderService = emailSenderService;
            _settingService = settingService;

            var siteSettings = GetGlobalSetting();
            LogoImageNameDark=siteSettings.LogoImageNameDark;
        }

        public async Task<bool> SendAdminUserForgotPasswordEmail(AdminUser user, string email)
        {
            bool hasSend = false;
            try
            {
                var callbackUrl = $"/Account/ResetPassword/{user.ForgotPasswordLink}";
                var urlToClick = "<a  oncontextmenu:'return false' href='" + SiteKeys.Domain + callbackUrl + "' target='_blank'>click here</a>";

                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Europe Trip: Reset Password";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/logo.png\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                //    " <p>You recently requested to reset the password for your account. Here&#39;s your&nbsp;link to do so: @@RESETLINK@@</p>    <p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";
                emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                    " <p>You recently requested to reset the password for your account. Here&#39;s your&nbsp;link to do so: @@RESETLINK@@</p>    <p>Kind regards,</p>   " +
                    " <p><strong>Europe Trip</strong></p>";
                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", user.FirstName + " " + user.LastName)
                        .Replace(@"@@RESETLINK@@", urlToClick);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
                    //hasSend = await _emailSenderService.SendEmailByPostmarkAPI(email, emailTemplate.Subject, body, emailTemplate, SiteKeys.AdminDisplayName);
                }


            }
            catch
            {
            }
            return hasSend;
        }
        public async Task<bool> SendUserForgotPasswordEmail(User user, string email)
        {
            bool hasSend = false;
            try
            {
                var callbackUrl = $"/Account/ResetPassword/{user.ForgotPasswordLink}";
                var urlToClick = "<a  oncontextmenu:'return false' href='" + SiteKeys.Domain + callbackUrl + "' target='_blank'>click here</a>";

                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Europe Trip: Reset Password";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                //    " <p>You recently requested to reset the password for your account. Here&#39;s your&nbsp;link to do so: @@RESETLINK@@</p>    <p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";
                emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                   " <p>You recently requested to reset the password for your account. Here&#39;s your&nbsp;link to do so: @@RESETLINK@@</p>    <p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", user.FirstName + " " + user.LastName)
                        .Replace(@"@@RESETLINK@@", urlToClick);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
                    //hasSend = await _emailSenderService.SendEmailByPostmarkAPI(email, emailTemplate.Subject, body, emailTemplate, SiteKeys.AdminDisplayName);
                }


            }
            catch
            {
            }
            return hasSend;
        }
        public async Task<bool> SendOTPEmail(User user)
        {
            bool hasSend = false;
            try
            {
                var otp = PasswordEncryption.GenerateRandomOTP();

                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Europe Trip: OTP (One Time Password)";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                //    " <p>Your Verification OTP details is : <p>verified email : @@EMAIL@@</p> <p>Verification OTP : @@OTP@@</p>    <p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                   " <p>Your Verification OTP details is : <p>verified email : @@EMAIL@@</p> <p>Verification OTP : @@OTP@@</p>    <p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", GetFullName(user.FirstName ?? "", user.LastName ?? ""))
                        .Replace(@"@@EMAIL@@", user.Email)
                        .Replace(@"@@OTP@@", otp);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(user.Email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
                    //hasSend = await _emailSenderService.SendEmailByPostmarkAPI(email, emailTemplate.Subject, body, emailTemplate, SiteKeys.AdminDisplayName);
                    user.EmailOtp = otp;
                }


            }
            catch
            {
            }
            return hasSend;
        }

        public async Task<bool> SendCustomPlanEmail(User user, long id, char sendFrom, string generatedPwd, bool isNewUser)
        {
            bool hasSend = false;
            try
            {

                var callbackUrl = $"/Account/CheckActivation/?uid={user.Id}&reqid={id}&sendFrom={sendFrom}&docket={user.EmailVerificationToken}&isNewUser={isNewUser}";  //docket=token
                var urlToClick = "<a oncontextmenu:'return false' data-toggle=\"modal\" data-target=\"#login-popup\" href='" + SiteKeys.Domain + callbackUrl + "' target='_blank'>click here to Login</a>";
                var IsUserEmailVerified = user.IsEmailVerified == false ? "Here are your login details: \n <p>Username : @@EMAIL@@</p> <p>Password : @@Password@@</p>" : "";
                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Welcome to Europe Trip";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></a></p>    <p><strong>Welcome Onboard @@NAME@@</strong></p>   " +
                //    " <p>We'll help you plan a holiday exactly the way you want. @@IsUserEmailVerifiedHtml@@   \n  @@RESETLINK@@ <p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@LOGOSITEURL@@\" style=\"height:50px\" /></a></p>    <p><strong>Welcome Onboard @@NAME@@</strong></p>   " +
                   " <p>We'll help you plan a holiday exactly the way you want. @@IsUserEmailVerifiedHtml@@   \n  @@RESETLINK@@ <p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", SiteKeys.Domain)
                        .Replace(@"@@LOGOSITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", GetFullName(user.FirstName ?? "", user.LastName ?? ""))
                        .Replace(@"@@IsUserEmailVerifiedHtml@@", IsUserEmailVerified)
                        .Replace(@"@@EMAIL@@", user.Email)
                        .Replace(@"@@Password@@", generatedPwd)
                        .Replace(@"@@RESETLINK@@", urlToClick);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(user.Email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);

                }


            }
            catch
            {
            }
            return hasSend;
        }

        public async Task<bool> SendContactQueryReply(UserNotification notification, string contactEmail, string contactUser)
        {
            bool hasSend = false;
            try
            {
                var imagepath = SiteKeys.Domain + "/" +SiteKeys.UploadFilesQueryReply + "/" + notification.ImageName;
                var IsImageExists = string.IsNullOrEmpty(notification.ImageName) ? "<img alt=\"No Image\" src=\"" + SiteKeys.NoImagePath_Square + "\" style=\"height:200px\" />" : "<a href=\"" + imagepath + "\"><img alt=\"Europe Trip\" src=\"" + imagepath + "\" style=\"height:200px\" /></a>";
                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Europe Trip : Contact Query Reply";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></a></p>   " +
                //    " <p><strong>Hello @@NAME@@</strong></p> " +
                //    " <p>Travel admin reply for your contact query :  </p>" +
                //    "<p>@@QUERYIMG@@</p> " +
                //    " <p><strong>Title : </strong>@@TITLE@@</p> " +
                //    " <p><strong>Query Reply : </strong>@@QUERYREPLY@@</p> " +
                //    "<p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@LOGOSITEURL@@\" style=\"height:50px\" /></a></p>   " +
                   " <p><strong>Hello @@NAME@@,</strong></p> " +
                   " <p>Travel admin reply for your contact query :  </p>" +
                   "<p>@@QUERYIMG@@</p> " +
                   " <p><strong>Title : </strong>@@TITLE@@</p> " +
                   " <p><strong>Query Reply : </strong>@@QUERYREPLY@@</p> " +
                   "<p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", SiteKeys.FrontDomain)
                        .Replace(@"@@LOGOSITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", contactUser)
                        .Replace(@"@@QUERYIMG@@", IsImageExists)
                        .Replace(@"@@TITLE@@", notification.Title)
                        .Replace(@"@@QUERYREPLY@@", notification.Descriptions);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(contactEmail, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);

                }

            }
            catch
            {
            }
            return hasSend;
        }

        public async Task<bool> SendQuoteEmail(long QuoteFeedbackId, long QuoteForEnqueryId, long UserId, long PackageId, string QuoteVerificationToken, string UserName, string Email, int FeedbackForEnqueryType, string PackageUrlForQuote, string Subject, string EmailType, string EmailCustomContent, string TestField)
        {
            string MailBCC = SiteKeys.MailBCC;
            string AdminEmailAddress = SiteKeys.AdminEmailAddress;
            string AdminDisplayName = SiteKeys.AdminDisplayName;

            UserName = (UserName == null || UserName.Trim() == "") ? "" : " " + UserName.Trim();

            bool hasSend = false;
            try
            {

                var callbackUrl = $"/Account/QuoteFeedbackSubmit/?qfid={QuoteFeedbackId}&uid={UserId}&pkgid={PackageId}&typ={FeedbackForEnqueryType}&docket={QuoteVerificationToken}";  //docket=token
                var urlToClick = "click here to <a oncontextmenu:'return false' data-toggle=\"modal\" data-target=\"#login-popup\" href='" + SiteKeys.FrontDomain + callbackUrl + "' target='_blank'>submit your feedback</a> for the package. \n";
                var package_url_detail = " \n Here are your customize package URL: \n <p><a href=" + PackageUrlForQuote + " target='_blank'> " + PackageUrlForQuote + "</a></p>";

                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                //emailTemplate.Subject = "Welcome to Europe Trip";
                //emailTemplate.EmailType = 5;
                string enqueryTypemsg = "customize";
                if (FeedbackForEnqueryType == 1)
                {
                    enqueryTypemsg = "package";
                }


                emailTemplate.Subject = Subject;
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></a></p>    <p><strong>Welcome@@NAME@@,</strong></p>   " +
                //    " <p>We have created a new " + enqueryTypemsg + " plan as per your request id: " + QuoteForEnqueryId + ", please review it and provide your confirmation if all is as per your requirement, you can write us to include further changes in this customize package.</p> \n  <p> @@PackaeUrlDetailHtml@@  \n <p>" + EmailCustomContent + "</p>   \n  @@RESETLINK@@";
                //emailTemplate.EmailContent = emailTemplate.EmailContent + "   \n\n  <p>Kind regards,</p>  <p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip\" src=\"@@LOGOSITEURL@@\" style=\"height:50px\" /></a></p>    <p><strong>Welcome@@NAME@@,</strong></p>   " +
                   " <p>We have created a new " + enqueryTypemsg + " plan as per your request id: " + QuoteForEnqueryId + ", please review it and provide your confirmation if all is as per your requirement, you can write us to include further changes in this customize package.</p> \n  <p> @@PackaeUrlDetailHtml@@  \n <p>" + EmailCustomContent + "</p>   \n  @@RESETLINK@@";
                emailTemplate.EmailContent = emailTemplate.EmailContent + "   \n\n  <p>Kind regards,</p>  <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model

                if (emailTemplate != null)
                {
                    string siteDomin = SiteKeys.Domain;
                    if (TestField == "1")
                    {
                        emailTemplate.EmailContent = "Not emailTemplate.EmailContent";
                    }
                    else if (TestField == "2")
                    {
                        siteDomin = "Not SiteKeys.Domain";
                    }
                    else if (TestField == "3")
                    {
                        UserName = "Not UserName";
                    }
                    else if (TestField == "4")
                    {
                        package_url_detail = "Not package_url_detail";
                    }
                    else if (TestField == "5")
                    {
                        urlToClick = "urlToClick";
                    }




                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", siteDomin)
                         .Replace(@"@@LOGOSITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", UserName)
                        .Replace(@"@@PackaeUrlDetailHtml@@", package_url_detail)
                        .Replace(@"@@RESETLINK@@", urlToClick);
                    //  .Replace(@"@@EMAIL@@", Email);

                    body = body.Replace("@@PackaeUrlDetailHtml@@", package_url_detail);
                    if (TestField == "6")
                    {
                        emailTemplate.Subject = "Not subject";
                    }
                    else if (TestField == "7")
                    {
                        body = "Not body";
                    }


                    //string sanitizedBody = Regex.Replace(body, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
                    //sanitizedBody = Regex.Replace(sanitizedBody, @"on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);

                    //string sanitizedSubject = Regex.Replace(emailTemplate.Subject, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
                    //// Remove any on* event attributes (e.g., onclick, onmouseover)
                    //sanitizedSubject = Regex.Replace(sanitizedSubject, @"on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);


                   string sanitizedBody = sanitizedTextContent(body);
                   string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(Email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, MailBCC, AdminEmailAddress, AdminDisplayName);

                    WriteLogFile("Log Date Time: " + DateTime.Now.ToString());
                    WriteLogFile("Email Sent Successfully: " + hasSend.ToString());
                    WriteLogFile("Email Content: " + body.ToString());

                }


            }
            catch (Exception ex)
            {
                WriteLogFile("Log Error Date Time: " + DateTime.Now.ToString());
                WriteLogFile("Exception StackTrace: " + ex.StackTrace);
                WriteLogFile("Exception InnerException: " + ex.InnerException);
                WriteLogFile("Exception Message: " + ex.Message.ToString());
            }
            return hasSend;
        }
        private static void WriteLogFile(string Description)
        {

            string filePath = string.Concat(SiteKeys.AdminEmailLogs, "Email.txt");

            System.IO.StreamWriter objWrite = default(System.IO.StreamWriter);

            if (System.IO.File.Exists(filePath))//append
            {
                objWrite = System.IO.File.AppendText(filePath);
            }
            else//create                
                objWrite = System.IO.File.CreateText(filePath);

            objWrite.WriteLine(Description + System.Environment.NewLine);

            objWrite.Flush();
            objWrite.Close();
            objWrite.Dispose();
        }
        private async Task<bool> SendEmail(string to, string subject, string message, EmailTemplate emailTemplate, string cc = null, string bcc = null, string from = null, string fromDisplayName = null, string[] attachment = null, string replyToEmails = null)
        {
            try
            {
                string toEmails = to;
                string ccEmails = cc;
                string bccEmails = bcc;

                if (!string.IsNullOrEmpty(emailTemplate.To))
                {
                    toEmails = toEmails != null ? $"{toEmails};{emailTemplate.To}" : emailTemplate.To;
                }

                if (!string.IsNullOrEmpty(emailTemplate.Cc))
                {
                    ccEmails = ccEmails != null ? $"{ccEmails};{emailTemplate.Cc}" : emailTemplate.Cc;
                }

                if (!string.IsNullOrEmpty(emailTemplate.Bcc))
                {
                    bccEmails = bccEmails != null ? $"{bccEmails};{emailTemplate.Bcc}" : emailTemplate.Bcc;
                }
                //WriteLog.Log(LogSection.Email, "Start SendEmail - To - " + to + " Subject -" + subject);
                await _emailSenderService.SendEmailAsync(ToEmails: toEmails, subject: subject ?? emailTemplate.Subject, message: message, ccEmails: ccEmails, from: from, bccEmails: bccEmails, fromDisplayName: fromDisplayName, replyToEmails: replyToEmails);
                //WriteLog.Log(LogSection.Email, "End SendEmail Success ");
            }
            catch (Exception ex)
            {
                //WriteLog.Log(LogSection.Email, $"Error SendEmail - {ex.Message}-{ex.InnerException?.Message}-{ex.StackTrace}");
                return false;
            }
            return true;
        }
        public static string GetFullName(string firstName, string lastName)
        {
            return firstName + " " + lastName;
        }

        public async Task<bool> SendVerificationEmail(User user, char sendFrom, string password, bool isNewUser)
        {
            bool hasSend = false;
            try
            {

                var callbackUrl = $"/Account/CheckActivation/?uid={user.Id}&reqid={0}&sendFrom={sendFrom}&docket={user.EmailVerificationToken}&isNewUser={isNewUser}";  //docket=token
                var urlToClick = "<a oncontextmenu:'return false' data-toggle=\"modal\" data-target=\"#login-popup\" href='" + SiteKeys.Domain + callbackUrl + "' target='_blank'>click here to verify</a>";
                //   var IsUserEmailVerified = user.IsEmailVerified == false ? "Here are your login details: \n <p>Username : @@EMAIL@@</p> <p>Password : @@Password@@</p>" : "";
                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Welcome to Europe Trip";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/logo.png\" style=\"height:50px\" /></p>    <p><strong>Welcome Onboard @@NAME@@</strong></p>   " +
                //    " <p>Please verify your account.</p> \n <p> @@RESETLINK@@ <p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";
                emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@\" style=\"height:50px\" /></p>    <p><strong>Welcome Onboard @@NAME@@</strong></p>   " +
                   " <p>Please verify your account.</p> \n <p> @@RESETLINK@@ <p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@SITEURL@@", LogoImageNameDark)
                        .Replace(@"@@NAME@@", GetFullName(user.FirstName ?? "", user.LastName ?? ""))
                        ////  .Replace(@"@@IsUserEmailVerifiedHtml@@", IsUserEmailVerified)
                        ////  .Replace(@"@@EMAIL@@", user.Email)
                        ////  .Replace(@"@@Password@@", password)
                        .Replace(@"@@RESETLINK@@", urlToClick);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(user.Email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);

                }


            }
            catch
            {
            }
            return hasSend;


            //bool hasSend = false;
            //try
            //{
            //    //////var otp = PasswordEncryption.GenerateRandomOTP();

            //    //////#region Bind Email Model
            //    //////EmailTemplate emailTemplate = new EmailTemplate();
            //    //////emailTemplate.Subject = "Europe Trip: OTP (One Time Password)";
            //    //////emailTemplate.EmailType = 5;
            //    //////emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/logo.png\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
            //    //////    " <p>Your Verification OTP details is : <p>verified email : @@EMAIL@@</p> <p>Verification OTP : @@OTP@@</p>    <p>Kind regards,</p>   " +
            //    //////    " <p><strong>Europe Trip</strong></p>";
            //    //////emailTemplate.IsActive = true;
            //    //////#endregion Bind Email Model
            //    //////if (emailTemplate != null)
            //    //////{
            //    //////    string body = emailTemplate.EmailContent
            //    //////        .Replace(@"@@SITEURL@@", SiteKeys.Domain)
            //    //////        .Replace(@"@@NAME@@", GetFullName(user.FirstName ?? "", user.LastName ?? ""))
            //    //////        .Replace(@"@@EMAIL@@", user.Email)
            //    //////        .Replace(@"@@OTP@@", otp);

            //    //////    hasSend = await SendEmail(user.Email, emailTemplate.Subject, body, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
            //    //////    //hasSend = await _emailSenderService.SendEmailByPostmarkAPI(email, emailTemplate.Subject, body, emailTemplate, SiteKeys.AdminDisplayName);
            //    //////    user.EmailOtp = otp;
            //    }


            //}
            //catch
            //{
            //}
            //return hasSend;
        }

        public async Task<bool> SendEmailBookingSuccess(BookingSuccessDto model)
        {
            bool hasSend = false;
            try
            {
                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Booking Confirmed - " + model.BookingId.ToString();
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/logo.png\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                //    "<p>Congratulations, your booking in Europe Trip has been successfull.</p>" +
                //    "<p>Here is your booking details:</p>" +
                //    "<p>Booking Number: @@BookingNo@@</p> " +
                //    "<p>Booking Date & Time: @@BookingDate@@</p> " +
                //    "<p>Booked Package Name: <a href=\"@@SITEURL@@/package/@@PackageId@@\">@@PackageName@@</a></p>" +
                //    "<p>Package Start Date: @@FromDate@@</p> " +
                //    "<p>Package End Date: @@ToDate@@</p> " +
                //    "<p><a href=\"@@SITEURL@@\">Click here</a> to login</p>" +
                //    "<p>Kind Regards,</p>   " +
                //    "<p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><img alt=\"Europe Trip\" src=\"@@LOGOSITEURL@@\" style=\"height:50px\" /></p>    <p>Hello @@NAME@@,</p>   " +
                   "<p>Congratulations, your booking in Europe Trip has been successfull.</p>" +
                   "<p>Here is your booking details:</p>" +
                   "<p>Booking Number: @@BookingNo@@</p> " +
                   "<p>Booking Date & Time: @@BookingDate@@</p> " +
                   "<p>Booked Package Name: <a href=\"@@SITEURL@@/package/@@PackageId@@\">@@PackageName@@</a></p>" +
                   "<p>Package Start Date: @@FromDate@@</p> " +
                   "<p>Package End Date: @@ToDate@@</p> " +
                   "<p><a href=\"@@SITEURL@@\">Click here</a> to login</p>" +
                   "<p>Kind Regards,</p>   " +
                   "<p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@LOGOSITEURL@@", LogoImageNameDark)
                        .Replace(@"@@SITEURL@@", SiteKeys.Domain)
                        .Replace(@"@@NAME@@", GetFullName(model.FirstName ?? "", model.LastName ?? ""))
                        .Replace(@"@@PackageName@@", model.PackageName)
                        .Replace(@"@@BookingNo@@", model.BookingId.ToString())
                        .Replace(@"@@FromDate@@", model.FromDate)
                        .Replace(@"@@ToDate@@", model.Todate)
                        .Replace(@"@@PackageId@@", model.PackageUrl)
                        .Replace(@"@@BookingDate@@", model.BookingDate);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(model.Email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);

                }


            }
            catch
            {
            }
            return hasSend;
        }

        //Arvind Kumar 28-11-2023
        public async Task<bool> SendFrontUserEmail(string msg, string email, string subject)
        {
            bool hasSend = false;
            try
            {
                #region Bind Email Model
                var callbackUrl = $"/Account/";
                var urlToClick = "<a  oncontextmenu:'return false' href='" + SiteKeys.FrontDomain + "' target='_blank'>click here</a>";

                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = subject;
                emailTemplate.EmailType = 5;
                emailTemplate.EmailContent = msg;
                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                       .Replace(@"@@SITEURL@@", SiteKeys.Domain)
                       .Replace(@"@@RESETLINK@@", urlToClick);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
                    // hasSend = await SendEmail(email, emailTemplate.Subject, body, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);
                }


            }
            catch
            {
            }
            return hasSend;
        }
        public async Task<bool> SendNotification(UserNotification userNotification, string userName, string email)
        {
            bool hasSend = false;
            try
            {
                var imagepath = SiteKeys.Domain + "/" + userNotification.ImageName;
                var IsImageExists = string.IsNullOrEmpty(userNotification.ImageName) ? "<img alt=\"No Image\" src=" + SiteKeys.NoImagePath_Square + " style=\"height:50px\" />" : "<a href=" + imagepath + "><img alt=\"Europe Trip Notification\" src=" + imagepath + " style=\"height:50px\" /></a>";
                #region Bind Email Model
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate.Subject = "Europe Trip : Notification";
                emailTemplate.EmailType = 5;
                //emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip Notification\" src=\"@@SITEURL@@/images/login-logo.png\" style=\"height:50px\" /></a></p>   " +
                //    " <p><strong>Hello @@NAME@@</strong></p> " +
                //    "<p>@@QUERYIMG@@</p> " +
                //    " <p><strong>Title : </strong>@@TITLE@@</p> " +
                //    " <p><strong>Description : </strong>@@Descriptions@@</p> " +
                //    "<p>Kind regards,</p>   " +
                //    " <p><strong>Europe Trip</strong></p>";

                emailTemplate.EmailContent = "<p><a href=\"@@SITEURL@@\"><img alt=\"Europe Trip Notification\" src=\"@@LOGOSITEURL@@\" style=\"height:50px\" /></a></p>   " +
                  " <p><strong>Hello @@NAME@@,</strong></p> " +
                  "<p>@@QUERYIMG@@</p> " +
                  " <p><strong>Title : </strong>@@TITLE@@</p> " +
                  " <p><strong>Description : </strong>@@Descriptions@@</p> " +
                  "<p>Kind regards,</p>   " +
                  " <p><strong>Europe Trip</strong></p>";

                emailTemplate.IsActive = true;
                #endregion Bind Email Model
                if (emailTemplate != null)
                {
                    string body = emailTemplate.EmailContent
                        .Replace(@"@@LOGOSITEURL@@", LogoImageNameDark)
                        .Replace(@"@@SITEURL@@", SiteKeys.FrontDomain)
                        .Replace(@"@@NAME@@", userName)
                        .Replace(@"@@QUERYIMG@@", IsImageExists)
                        .Replace(@"@@TITLE@@", userNotification.Title)
                        .Replace(@"@@Descriptions@@", userNotification.Descriptions);

                    string sanitizedBody = sanitizedTextContent(body);
                    string sanitizedSubject = sanitizedTextContent(emailTemplate.Subject);

                    hasSend = await SendEmail(email, sanitizedSubject, sanitizedBody, emailTemplate, string.Empty, SiteKeys.MailBCC, SiteKeys.AdminEmailAddress, SiteKeys.AdminDisplayName);

                }

            }
            catch
            {
            }
            return hasSend;

        }





        private GeneralSiteSettingDTO GetGlobalSetting()
        {
            var globalSetting = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
            GeneralSiteSettingDTO obj = new GeneralSiteSettingDTO();
            if (globalSetting != null)
            {
                obj.SupportEmail = globalSetting.SupportEmail;
                obj.SupportMobile = globalSetting.SupportMobile;
                obj.LogoImageName = SiteKeys.ImageDomain + SiteKeys.UploadFilesTheme + globalSetting.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.ImageDomain + SiteKeys.UploadFilesTheme + globalSetting.LogoImageNameDark;
            }
            else
            {
                obj.SupportEmail = SiteKeys.SupportEmail;
                obj.SupportMobile = SiteKeys.SupportMobile;
                obj.LogoImageName = SiteKeys.Domain + "/Image" + SiteKeys.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.Domain + "/Image" + SiteKeys.LogoImageNameDark;
            }
            return obj;
        }


        private string sanitizedTextContent(string textContent)
        {
            string sanitizedContent = Regex.Replace(textContent, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
            sanitizedContent = Regex.Replace(sanitizedContent, @"on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
            return sanitizedContent; 
        }

    }
}
