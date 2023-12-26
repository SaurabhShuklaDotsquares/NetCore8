using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Win32;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using TCP.Core;
using TCP.Core.Code.LIBS;
using TCP.Core.Code.Notification;
using TCP.Core.Models;
using TCP.Data.Models;
using TCP.Service;
using TCP.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCP.Web.Controllers
{
    public class CustomQuoteController : BaseController
    {
        private readonly IConfiguration _configuration;
        private IDataProtector _protector;
        private readonly ICustomQuoteService _customQuoteService;
        private readonly IUserService _userService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly INotificationService _notificationService;
        private readonly IPackageService _pkgservice;
        public CustomQuoteController(INotificationService notificationService, IConfiguration configuration, IDataProtectionProvider provider, ICustomQuoteService customQuoteService, IUserService userService, IEmailFactoryService emailFactoryService, IPackageService pkgservice)
        {
            _configuration = configuration;
            var protectorPurpose = "secure username and password";
            _protector = provider.CreateProtector(protectorPurpose);
            _customQuoteService = customQuoteService;
            _userService = userService;
            _emailFactoryService = emailFactoryService;
            _notificationService = notificationService;
            _pkgservice = pkgservice;
        }


        #region [CustomQuote]
        [HttpGet]
        public async Task<IActionResult> CustomQuote(string? flagfrom)
        {
            CustomQuoteViewModel planMyObj = new CustomQuoteViewModel();
            ViewBag.flagfrom = flagfrom ?? "";
            return PartialView("_CustomQuote", planMyObj);
        }

        [HttpPost]
        public async Task<IActionResult> CustomQuote(CustomQuoteViewModel modal)
        {
            try
            {
                string pkgid = "0";
                bool isNewUser = false;
                string WhenUserActive = string.Empty;
                string link_WhenUserActive = string.Empty;
                if (ModelState.IsValid)
                {
                    string displayMsg = string.Empty;
                    #region Create New USER
                    RegisterViewModel registerView = new RegisterViewModel();
                    registerView.Email = modal.Email;
                    registerView.MobilePhone = modal.PhoneNumber;

                    User usermodel = _userService.GetUserByEmail(registerView.Email);
                    if (usermodel == null && _userService.IsUserExists(modal.Email) == false)
                    {
                        usermodel = CommonFileViewModel.RegisterNewUser(registerView);
                        usermodel = await _userService.SaveUser(usermodel);

                        Billing billing = _userService.GetBillingByUserId(usermodel.Id) ?? new Billing();
                        if (billing?.UserId <= 0)
                        {
                            billing.UserId = usermodel.Id;
                            billing.CreatedBy = CurrentFrontUser.Id;
                            billing.CreationOn = DateTime.UtcNow;
                            await _userService.SaveBilling(billing);
                        }

                        HttpContext.Session.SetString("RegisteredPwd", registerView.Password);
                        isNewUser = true;
                    }
                    else
                    {
                        usermodel.MobilePhone = modal.PhoneNumber; // usermodel.MobilePhone ?? modal.PhoneNumber;
                        usermodel = await _userService.UpdateUser(usermodel);
                    }
                    #endregion

                    #region CustomizePackageRequest saving with user id

                    CustomizePackageRequest customreq = new CustomizePackageRequest();
                    Uri myUri = new Uri(modal.packageUrl);
                    string queryString = myUri.ToString();
                    if (string.IsNullOrEmpty(queryString) || !queryString.Contains("/package/"))
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please enter valid URL", IsSuccess = false });
                    }
                    int lastSlashIndex = queryString.LastIndexOf('/');
                    if (lastSlashIndex != -1)
                    {
                        // Get the substring from the last slash to the end
                        string desiredPart = queryString.Substring(lastSlashIndex + 1);
                        if (string.IsNullOrEmpty(desiredPart)||desiredPart.Contains("?pkgid")|| desiredPart.Contains("?")|| desiredPart.Contains("&"))
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please enter valid URL", IsSuccess = false });
                        }
                        pkgid = _pkgservice.GetActivePackageByUrl(desiredPart).Id.ToString();
                    }
                    //string pkgid = HttpUtility.ParseQueryString(myUri.Query).Get("pkgid");
                    if (usermodel != null && usermodel.Id > 0)
                    {
                        customreq.UserId = usermodel.Id;
                        customreq.PkgIdToCustomize = Convert.ToInt64(pkgid);
                        customreq.PkgUrlToCustomize = modal.packageUrl;
                        customreq.InfoToCustomize = modal.Requirement;
                        customreq.CustomizedPkgId = modal.CustomizedPkgId;
                        customreq.CustomizeRequestParentId = modal.CustomizeRequestParentId;
                        customreq.PackageVersion = 1;
                        customreq.IsNewRequest = true;
                        customreq.PackageRequestStatusId = (int)PackageRequestStatus.Pending;
                        customreq.CreationOn = DateTime.UtcNow;
                        customreq.CreatedBy = CurrentFrontUser.Id;
                        customreq = await _customQuoteService.SaveCustomizePackageRequest(customreq); //CustomizePackageRequest saving

                        #endregion

                        #region Send Email to user based on above ids for login  
                        if (customreq != null && customreq.Id > 0)
                        {
                            registerView.Password = string.IsNullOrEmpty(HttpContext.Session.GetString("RegisteredPwd")) ? "" : HttpContext.Session.GetString("RegisteredPwd");
                            if (string.IsNullOrEmpty(registerView.Password) || string.IsNullOrWhiteSpace(registerView.Password))
                            {
                                usermodel.SaltKey = PasswordEncryption.CreateSaltKey();
                                var password = PasswordEncryption.GenerateRandomPassword();

                                usermodel.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(password, usermodel.SaltKey);
                                await _userService.UpdateUser(usermodel);
                                registerView.Password = password;
                            }
                            WhenUserActive = usermodel.IsActive == false ? $" if you didn't receive any email? then please click on <span class='text-primary resendmailcls cursor-pointer'  data-href='/CustomQuote/ResendCustomPlanConfirmationLink?userid={usermodel.Id}&reqId={customreq.Id}&regPwd={registerView.Password}'>  Resend Confirmation Link</span>" : "";
                            link_WhenUserActive = usermodel.IsActive == false ? $" you will get a confirmation link on your registered email <b>{usermodel.Email}</b> in short while, please click on the link to activate your sign up request.{WhenUserActive}" : "your request has been sent to our sales executive, they will contact you soon on your email or phone.";

                            await Task.Run(() => { _emailFactoryService.SendCustomPlanEmail(usermodel, customreq.Id, 'C', registerView.Password, isNewUser); });
                            await _userService.UpdateUser(usermodel);

                            displayMsg = $"<b>Thank you for submitting your request.</b><br/> {link_WhenUserActive}";

                            //SaveUserNotification
                            CreateNotification createNotification = new CreateNotification();
                            UserNotification userNotification = createNotification.CreateUserNotification(1, 1, 5, CurrentFrontUser.Id, 1, "New customize quote enquiry submitted", "New customize quote enquiry id: " + customreq.Id.ToString() + " submitted successfully.");
                            await _notificationService.SaveUserNotification(userNotification);
                            //EndSaveUserNotification
                        }
                        #endregion
                    }

                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });

                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "model state not valid", IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.Message, IsSuccess = false });
            }
        }

        //Resend ConfirmationLink work
        // [HttpPost]
        public async Task<IActionResult> ResendCustomPlanConfirmationLink(int userid, int reqId, string regPwd)
        {
            string displayMsg = string.Empty;
            try
            {
                if (userid <= 0 && reqId <= 0 && regPwd == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Something went wrong", IsSuccess = false });

                }
                var usermodel = _userService.GetUserById(userid);
                if (usermodel != null && usermodel.IsActive == false && usermodel.IsDeleted == false)
                {
                    if (usermodel.Id > 0 && usermodel.IsEmailVerified == false)
                    {
                        usermodel.EmailVerificationToken = Guid.NewGuid().ToString();
                        usermodel.EmailVerificationOtpExpired = DateTime.Now.AddDays(30);
                        await _userService.UpdateUser(usermodel);
                        await Task.Run(() => { _emailFactoryService.SendCustomPlanEmail(usermodel, reqId, 'C', regPwd, true); });
                        usermodel.ModifiedOn = DateTime.Now;
                        await _userService.UpdateUser(usermodel);
                    }
                    displayMsg = $"<b>Thank you for submitting your request.</b> A confirmation link has been resent to your email {usermodel.Email}.Please click on the link to activate your request." +
                                $"Did't receive email? <span class='text-primary resendmailcls cursor-pointer' data-href='/CustomQuote/ResendCustomPlanConfirmationLink?userid={userid}&reqId={reqId}&regPwd={regPwd}'> Resend Confirmation Link</span>";

                    //displayMsg = $"Thank you for submitting your request. A confirmation link has been resent to your email {usermodel.Email}.";
                    //ShowSuccessMessage("Success", displayMsg, false);
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });



                    ////// displayMsg = $"Thank you for submitting your request. A confirmation link has been resent to your email {usermodel.Email}.";
                    //////ShowSuccessMessage("Success", displayMsg, false);
                }
                else if (usermodel.IsActive == true && usermodel.IsDeleted == false)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Your request has been confirmed already, Please Login", IsSuccess = false });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "User does't exists , Please Register", IsSuccess = false });
                    //ShowErrorMessage("Error", "User does't exists , Please Register", false);
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some Error Occurred please try after some time.", IsSuccess = false });
                //// ShowErrorMessage("Error", "Some Error Occurred please try after some time.", false);
            }

        }
        #endregion

    }
}
