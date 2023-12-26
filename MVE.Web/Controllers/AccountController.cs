using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using System;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
using TCP.Core;
using TCP.Core.Code.LIBS;
using TCP.Core.Code.Notification;
using TCP.Core.Models;
using TCP.Core.Models.Others;
using TCP.Data.Models;
using TCP.Dto;
using TCP.Service;
using TCP.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCP.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private IDataProtector _protector;
        private readonly IUserService _userService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly ICustomQuoteService _customQuoteService;

        public readonly IQuoteFeedbackService _quoteFeedbackService;
        public readonly IPlanHolidayService _repoNewPkgReq;
        public readonly IUserService _repoUser;
        private readonly IPackageService _packageService;
        private readonly INotificationService _notificationService;

        private readonly ISettingService _settingService;
        public AccountController(INotificationService notificationService, IConfiguration configuration, IDataProtectionProvider provider, IUserService userService, IEmailFactoryService emailFactoryService,
            ICustomQuoteService customQuoteService, IQuoteFeedbackService quoteFeedbackService, IPlanHolidayService repoNewPkgReq, IUserService repoUser,
            IPackageService packageService, ISettingService settingService)
        {
            _configuration = configuration;
            var protectorPurpose = "secure username and password";
            _protector = provider.CreateProtector(protectorPurpose);
            _userService = userService;
            _emailFactoryService = emailFactoryService;

            _quoteFeedbackService = quoteFeedbackService;
            _repoNewPkgReq = repoNewPkgReq;
            _repoUser = repoUser;
            _packageService = packageService;
            _customQuoteService = customQuoteService;
            _notificationService = notificationService;
            _settingService = settingService;
        }


        #region [LogIn]

        // verify user existens and set active
        public IActionResult CheckActivation(int? uid, int? reqid, char? sendFrom, string? docket, bool isNewUser = false)
        {
            bool status = false;
            if (uid > 0 && reqid > 0 && sendFrom != ' ' && !string.IsNullOrEmpty(docket))
            {
                var userDtls = _userService.GetUserById(uid ?? 0);

                //if (isNewUser == true)
                //{
                if (userDtls != null && !userDtls.IsDeleted && !userDtls.IsActive)
                {
                    if (!string.IsNullOrEmpty(userDtls.EmailVerificationToken) && docket == userDtls.EmailVerificationToken)
                    {
                        userDtls.IsEmailVerified = true;
                        userDtls.IsActive = true;
                        status = true;
                        userDtls.EmailVerificationOtpExpired = DateTime.UtcNow;
                        _userService.UpdateUser(userDtls);
                    }
                    else
                    {
                        SignOut();
                        ShowErrorMessage("Error", "Please Use Latest Link, Previous link has been expired", false);
                        return RedirectToAction("Index", "Home", new { reqid = reqid });
                    }
                }
                else if (userDtls.IsActive == true && userDtls.IsDeleted == false)
                {
                    SignOut();
                    ShowErrorMessage("Error", "Already Activated..", false);
                    return RedirectToAction("Index", "Home", new { reqid = reqid });
                }
                else
                {
                    SignOut();
                    ShowErrorMessage("Error", "Link Expired..", false);
                    return RedirectToAction("Index", "Home", new { reqid = reqid });
                }
                //}
                //else
                //{
                //if (userDtls != null && !userDtls.IsDeleted && !userDtls.IsActive)
                //{
                //    if (!string.IsNullOrEmpty(userDtls.EmailVerificationToken) && docket == userDtls.EmailVerificationToken)
                //    {
                //        userDtls.IsEmailVerified = true;
                //        userDtls.IsActive = true;
                //        status = true;
                //        userDtls.EmailVerificationOtpExpired = DateTime.UtcNow;
                //        _userService.UpdateUser(userDtls);
                //    }
                //}
                //  }
            }
            else if (uid > 0 && reqid == 0 && sendFrom != ' ')
            {
                if (isNewUser == true)
                {
                    var userDtls = _userService.GetUserById(uid ?? 0);
                    if (userDtls != null && !userDtls.IsDeleted && !userDtls.IsActive)
                    {
                        userDtls.IsEmailVerified = true;
                        userDtls.IsActive = true;
                        status = true;
                        _userService.UpdateUser(userDtls);
                        ShowSuccessMessage("Success", "Verification successfully. Please Login.", false);
                        return RedirectToAction("Index", "Home", new { reqid = reqid });
                    }
                    else
                    {
                        SignOut();
                        ShowErrorMessage("Error", "Link Expired..", false);
                        return RedirectToAction("Index", "Home", new { reqid = reqid });
                    }
                }


            }
            return RedirectToAction("Index", "Home", new { reqid = reqid });
        }
        public async Task<IActionResult> Login()
        {
            if (CurrentFrontUser.Id > 0)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            var model = new LoginViewModel();
            var userKey = Request.Cookies["FrontUserKey"];
            var userPwd = Request.Cookies["FrontUserValue"];
            if (!string.IsNullOrEmpty(userKey) && !string.IsNullOrEmpty(userPwd))
            {
                string unProtName = string.Empty;
                string unProtPwd = string.Empty;
                try
                {
                    unProtName = _protector.Unprotect(userKey);
                    unProtPwd = _protector.Unprotect(userPwd);
                    if (!string.IsNullOrEmpty(unProtName) && !string.IsNullOrEmpty(unProtPwd))
                    {
                        model.Email = unProtName;
                        model.Password = unProtPwd;
                        if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Email))
                        {
                            model.RememberMe = true;
                            await SignOut();
                            var isRedirect = await loginAsync(model);
                            if (string.IsNullOrEmpty(isRedirect))
                            {
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else
                            {
                                return View(model);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Response.Cookies.Delete("FrontUserKey");
                    Response.Cookies.Delete("FrontUserValue");
                }
            }
            else
            {
                await SignOut();
            }

            var globalSetting = GetGlobalSetting();
            model.SupportEmail = globalSetting.SupportEmail;
            model.SupportMobile = globalSetting.SupportMobile;
            model.LogoImageName = globalSetting.LogoImageName;
            model.LogoImageNameDark = globalSetting.LogoImageNameDark;

            return PartialView("_Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var validMessage = await loginAsync(model);
            if (validMessage!="")            
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = validMessage, IsSuccess = false });
            else
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> {Message= "Success", IsSuccess = true });
        }
        private async Task<string> loginAsync(LoginViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    model.Email = model.Email.Trim();
                    model.Password = model.Password.Trim();
                    var user = _userService.GetUserByEmail(model.Email);
                    if (user != null)
                    {
                        if (!user.IsDeleted)
                        {
                            if (user.IsActive)
                            {
                                var match = false;
                                match = PasswordEncryption.IsPasswordMatch(user.EncryptedPassword, model.Password, user.SaltKey);                               
                                if (match)
                                {
                                    //var roleId = user.RoleId;
                                    //var roleType = user.Role.RoleName;
                                    
                                    //var GlobalSetting = GetGlobalSetting();
                                    await CreateAuthenticationTicketUser(user, model.RememberMe);
                                    var cookieOptions = new CookieOptions
                                    {
                                        Expires = DateTimeOffset.UtcNow.AddDays(1),
                                        IsEssential = true,
                                        Domain=SiteKeys.Domain
                                    };
                                    if (model.RememberMe)
                                    {
                                        var protectedName = _protector.Protect(model.Email.Trim());
                                        var protectedPassword = _protector.Protect(model.Password);
                                        Response.Cookies.Append("FrontUserKey", protectedName, cookieOptions);
                                        Response.Cookies.Append("FrontUserValue", protectedPassword, cookieOptions);
                                    }
                                    else
                                    {
                                        Response.Cookies.Delete("FrontUserKey");
                                        Response.Cookies.Delete("FrontUserValue");
                                    }
                                    List<MenuItem> menuItems = new List<MenuItem>();
                                    //List<MenuViewModel> menuItemsList = new List<MenuViewModel>();
                                    //menuItems = _permissionService.GetAllMenus();

                                    //MenuViewModel.MenuItems = menuItems.Select(item => new MenuViewModel
                                    //{
                                    //    MenuName = item.Name,
                                    //    SubMenuName = item.SubMenuName,
                                    //    MenuIcon = item.Icon,
                                    //    SubMenuIcon = item.SubMenuIcon,
                                    //    MenuUrl = item.Url,
                                    //    IsActive = item.IsActive.Value,
                                    //    PermissionId = item.PermissionId.Value
                                    //}).ToList();

                                    displayMsg = string.Empty;
                                }
                                else
                                {
                                    displayMsg = "Password is incorrect";
                                }
                            }
                            else
                            {
                                displayMsg = "Your account is not active. please contact to administrator.";
                            }
                        }
                        else
                        {
                            displayMsg = "User is not available ! If you are registered please contact to administrator.";
                        }
                    }
                    else
                    {
                        displayMsg = "The entered email is not registered";
                    }
                }

                else
                {
                    displayMsg = "Invalid login attempt.";
                }
            }
            catch (Exception ex)
            {
                displayMsg = "Some Error Occurred please try after some time.";
            }

            return displayMsg;
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


        #endregion


        #region [Sign Up]
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();
            var globalSetting = GetGlobalSetting();
            model.SupportEmail = globalSetting.SupportEmail;
            model.SupportMobile = globalSetting.SupportMobile;
            model.LogoImageName = globalSetting.LogoImageName;
            model.LogoImageNameDark = globalSetting.LogoImageNameDark;


            return PartialView("_Register", model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile formFile)
        {
            bool isNewUser = false;
            string displayMsg = string.Empty;
            try
            {
                if (model.Terms_Conditions == false)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please Check the Term & Privacy Policy", IsSuccess = false });
                }
                else
                {
                    model.Email = model.Email.Trim();
                    var user = _userService.GetUserByEmail(model.Email);

                    if (user != null)
                    {
                        displayMsg = "User is already registered";
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
                    }
                    else
                    {
                        User usermodel = new User();
                        usermodel.FirstName = model.FullName.Trim();
                        usermodel.LastName = (model.LastName ?? "").Trim();
                        usermodel.Email = model.Email;
                        usermodel.DateOfBirth = model.DateOfBirth;
                        usermodel.SaltKey = PasswordEncryption.CreateSaltKey();
                        usermodel.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.Password, usermodel.SaltKey, "MD5");  //Encoding.UTF8.GetBytes(model.Password);
                        usermodel.Address = model.Address?.Trim();
                        usermodel.City = model.City?.Trim();
                        usermodel.CountryId = model.CountryId;
                        usermodel.MobilePhone = model.MobilePhone?.Trim();
                        usermodel.Gender = model.Gender;
                        usermodel.ZipCode = model.ZipCode?.Trim();
                        usermodel.StateOrCounty = model.StateOrCounty;
                        usermodel.IsEmailVerified = false;
                        usermodel.IsDeleted = false;
                        usermodel.IsActive = false;
                        usermodel.EmailVerificationOtpExpired = DateTime.Now.AddDays(-24);
                        usermodel.CreationOn = DateTime.UtcNow;
                        usermodel.CreatedBy = CurrentFrontUser.Id;
                        isNewUser = true;
                        if (formFile?.FileName != null)
                        {
                            usermodel.ImageName = formFile?.FileName;
                        }
                        else
                        {
                            usermodel.ImageName = "";
                        }

                        var newuser = await _userService.SaveUser(usermodel);


                        if (newuser != null && newuser.Id > 0 && newuser.IsEmailVerified == false)
                        {
                            if (formFile?.FileName != null)
                            {
                                CommonFileViewModel.FileUpload(formFile, SiteKeys.UploadFilesCountry);
                            }

                            //SaveUserNotification
                            CreateNotification createNotification = new CreateNotification();
                            UserNotification userNotification = createNotification.CreateUserNotification(1,1, 1, newuser.Id,1, "User Registration","Congratulations, Your registration on Europe Trip successfull.");
                            await _notificationService.SaveUserNotification(userNotification);
                            //EndSaveUserNotification

                            Billing billing = _userService.GetBillingByUserId(newuser.Id) ?? new Billing();
                            if (billing?.UserId <= 0)
                            {
                                billing.UserId = newuser.Id;
                                billing.CreatedBy = newuser.Id;
                                billing.CreationOn = DateTime.UtcNow;
                                await _userService.SaveBilling(billing);
                            }
                            await Task.Run(() => { _emailFactoryService.SendVerificationEmail(newuser, 'C', model.Password, isNewUser); });
                            await _userService.UpdateUser(usermodel);
                        }

                        ////displayMsg = "OTP Sent Successfully on registered email";
                        ////return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true, Data = newuser });

                        //ShowSuccessMessage("Success", "Verification link has been sent successfully to your registered Email address. Please click on that link and verify your account", false);
                        displayMsg = "Verification link has been sent successfully to your registered Email address. Please click on that link and verify your account.";
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
                        //return RedirectToAction("Index", "Home");
                    }

                }


            }
            catch (Exception ex)
            {
                displayMsg = "Some Error Occurred please try after some time.";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
            }
        }


        #endregion


        #region [VerifyOTP]
        [HttpGet]
        public IActionResult VerifyOTP(int userid)
        {
            var model = new RegisterViewModel();
            model.Id = userid;
            return PartialView("_VerifyOTP", model);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(RegisterViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                var user = _userService.GetUserById(model.Id);
                if (user != null && user.IsActive == false && user.IsDeleted == false)
                {
                    if (user.EmailOtp != model.EmailOtp)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Invalid OTP!", IsSuccess = false });
                    }
                    user.IsActive = true;
                    user.IsEmailVerified = true;
                    await _userService.UpdateUser(user);

                    displayMsg = "OTP Verified,You are Registered Successfully";
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
                }
                else
                {
                    displayMsg = "User does't exists , Please Register";
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
                }

            }
            catch (Exception ex)
            {
                displayMsg = "Some Error Occurred please try after some time.";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
            }
        }


        #endregion

        //resend OTP
        [HttpPost]
        public async Task<IActionResult> ResendOTP(int userid)
        {
            string displayMsg = string.Empty;
            try
            {
                if (userid <= 0)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Something went wrong", IsSuccess = false });

                }
                var user = _userService.GetUserById(userid);
                if (user != null && user.IsActive == false && user.IsDeleted == false)
                {
                    if (user.Id > 0 && user.IsEmailVerified == false)
                    {
                        await _emailFactoryService.SendOTPEmail(user);
                        await _userService.UpdateUser(user);
                    }
                    displayMsg = "OTP resent Successfully on registered email, Please check your email";
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true, Data = user });
                }
                else
                {
                    displayMsg = "User does't exists , Please Register";
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
                }

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some Error Occurred please try after some time.", IsSuccess = false });
            }
        }


        //[HttpGet]
        //public async Task<IActionResult> FeedbackOnQuote(long pkgid,long )
        //{
        //    string displayMsg = string.Empty;

        //}

        //[HttpPost]
        //public async Task<IActionResult> FeedbackOnQuote(int userid)
        //{
        //    string displayMsg = string.Empty;
        //    try
        //    {
        //        if (userid <= 0)
        //        {
        //            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Something went wrong", IsSuccess = false });

        //        }
        //        var user = _userService.GetUserById(userid);
        //        if (user != null && user.IsActive == false && user.IsDeleted == false)
        //        {
        //            if (user.Id > 0 && user.IsEmailVerified == false)
        //            {
        //                await _emailFactoryService.SendOTPEmail(user);
        //                await _userService.UpdateUser(user);
        //            }
        //            displayMsg = "OTP resent Successfully on registered email, Please check your email";
        //            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true, Data = user });
        //        }
        //        else
        //        {
        //            displayMsg = "User does't exists , Please Register";
        //            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some Error Occurred please try after some time.", IsSuccess = false });
        //    }
        //}



        #region Sign Out
        /// <summary>
        /// Represents an event that is raised when the sign-out operation is complete.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SignOut()
        {
            await signOutFunctionality();
            Response.Cookies.Delete("FrontUserKey");
            Response.Cookies.Delete("FrontUserValue");
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Remove user logged Info's from Cookies
        /// </summary>
        public async Task signOutFunctionality()
        {
            await RemoveAuthentication();
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddSeconds(1) };
            Response.Cookies.Append("UserSessionCookies", string.Empty, cookieOptions);
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
        }
        #endregion Sign Out

        #region [Forgot Password]
        [HttpGet]
        public IActionResult ForgotPassword()
        {            
            var model = new ForgetPasswordViewModel();
            var globalSetting = GetGlobalSetting();
            model.SupportEmail = globalSetting.SupportEmail;
            model.SupportMobile = globalSetting.SupportMobile;
            model.LogoImageName = globalSetting.LogoImageName;
            model.LogoImageNameDark = globalSetting.LogoImageNameDark;

            return PartialView("_ForgotPassword", model);
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                var user = _userService.GetUserByEmail(model.Email);
                if (user != null)
                {
                    if (!user.IsDeleted)
                    {
                        user.ForgotPasswordLink = Guid.NewGuid().ToString();
                        user.ForgotPasswordLinkExpired = DateTime.Now.AddDays(30);
                        user.ForgotPasswordLinkUsed = false;
                        user.ModifiedOn = DateTime.Now;
                        await _userService.UpdateUser(user);


                        await Task.Run(() => { _emailFactoryService.SendUserForgotPasswordEmail(user, model.Email); });
                        //if (await _emailFactoryService.SendUserForgotPasswordEmail(user, model.Email))
                        //{
                        displayMsg = "Reset password link has been sent successfully to your registered Email address.";
                        // return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true, Data = user });
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
                        //}
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "User doesn't Exists, please contact to administrator", IsSuccess = false });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "User doesn't Exists, please contact to administrator", IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                displayMsg = "Some Error Occurred please try after some time.";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = displayMsg, IsSuccess = false });
            }
        }
        //[HttpGet]
        //[Route("[Controller]/ForgotPassword")]
        //public IActionResult ForgotPassword()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var user = _userService.GetUserByEmail(model.Email);
        //            if (user != null)
        //            {
        //                if (!user.IsDeleted)
        //                {
        //                    user.ForgotPasswordLink = Guid.NewGuid().ToString();
        //                    user.ForgotPasswordLinkExpired = DateTime.Now.AddDays(30);
        //                    user.ForgotPasswordLinkUsed = false;
        //                    user.ModifiedOn = DateTime.Now;
        //                    await _userService.UpdateUser(user);
        //                    if (await _emailFactoryService.SendUserForgotPasswordEmail(user, model.Email))
        //                    {
        //                        ShowSuccessMessage("Success", "Reset password link has been sent successfully to your registered Email address.", false);
        //                        return RedirectToAction("Index");
        //                    }
        //                }
        //                else
        //                {
        //                    ShowErrorMessage("Error!", "Please provide valid Email address!!", false);
        //                    return RedirectToAction("ForgotPassword");
        //                }
        //            }
        //            else
        //            {
        //                ShowErrorMessage("Error!", "Please provide valid Email address!!", false);
        //                return RedirectToAction("ForgotPassword");
        //            }
        //        }
        //        else
        //        {
        //            ShowErrorMessage("Error!", "Didn't find any email address to send the reset password link", false);
        //            return RedirectToAction("ForgotPassword");
        //        }

        //    }
        //    catch
        //    {
        //        ShowErrorMessage("Error!", "Something went wrong with the process", false);
        //        return RedirectToAction("ForgotPassword");
        //    }

        //    ModelState.Clear();
        //    model.Email = string.Empty;
        //    return View(model);
        //}

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IActionResult ResetPassword(string id)
        {
            try
            {
                var user = _userService.GetUserByGuid(id);
                if (user != null)
                {
                    if (user.ForgotPasswordLinkExpired != null && user.ForgotPasswordLinkExpired >= DateTime.Now)
                    {
                        return View(new ResetPasswordViewModel { Id = user.ForgotPasswordLink });
                    }
                }

                return View(new ResetPasswordViewModel { Id = "" });
            }
            catch (Exception ex)
            {
                return View(new ResetPasswordViewModel { Id = "0" });
            }

        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userDtls = _userService.GetUserByGuid(model.Id);
                    if (userDtls != null)
                    {
                        if (!userDtls.IsDeleted)
                        {
                            userDtls.ForgotPasswordLink = null;
                            userDtls.ForgotPasswordLinkExpired = null;
                            userDtls.ForgotPasswordLinkUsed = true;
                            userDtls.SaltKey = PasswordEncryption.CreateSaltKey();
                            userDtls.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.Password, userDtls.SaltKey);
                            await _userService.UpdateUser(userDtls);
                            ShowSuccessMessage("Success", "Your password has been reset successfully, please login to continue", false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            ShowErrorMessage("Error", "User is not registered with this email.", false);
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Error", "Incorrect Old Password", false);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.Message, false);
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion





        #region [QuoteView]
        [HttpGet]
        public IActionResult QuoteFeedbackSubmit(int? qfid, long? uid, long? pkgid, int typ, string? docket)
        {
            var newPKG = _packageService.GetActivePackageById(Convert.ToInt64(pkgid));

            if (uid > 0 && pkgid > 0)
            {
                if (newPKG.QuoteVerificationOtpExpired != null)
                {
                    if ((!string.IsNullOrEmpty(newPKG.QuoteVerificationToken) && docket == newPKG.QuoteVerificationToken) == false)
                    {
                        ShowErrorMessage("Error", "Link has been expired", false);
                        return RedirectToAction("Index", "Home", new { pkgid = pkgid });
                    }
                }
                else
                {
                    ShowErrorMessage("Error", "Link Expired..", false);
                    return RedirectToAction("Index", "Home", new { pkgid = pkgid });
                }
            }


            QuoteFeedbackViewModel modal = new QuoteFeedbackViewModel();
            QuoteFeedbackForModel modalchild = new QuoteFeedbackForModel();

            NewPackageRequest repoNewPlanMyHoliday = null;
            CustomizePackageRequest repoNewCustomQuote = null;

            modal.PackageName = newPKG.Name;
            modal.PackageId = newPKG.Id;

            if (newPKG.QuoteForEnqueryType == 0)
            {
                int cprid = Convert.ToInt32(newPKG.QuoteForEnqueryId);
                repoNewCustomQuote = _customQuoteService.GetCustomizePackageRequestById(cprid);
                modal.IsQuoteFullfillRequirement = repoNewCustomQuote.PackageRequestStatusId == (int)PackageRequestStatus.Completed ? true : false;
                modal.IsItRequiredMoreChanges = repoNewCustomQuote.PackageRequestStatusId == (int)PackageRequestStatus.Completed ? false : true;
                modal.IsQuoteFullfillRequirementVal = repoNewCustomQuote.PackageRequestStatusId == (int)PackageRequestStatus.Completed ? "1" : "0";
                modal.IsItRequiredMoreChangesVal = repoNewCustomQuote.PackageRequestStatusId == (int)PackageRequestStatus.Completed ? "0" : "1";
            }
            else
            {
                repoNewPlanMyHoliday = _repoNewPkgReq.GetNewPackageRequestById(Convert.ToInt32(newPKG.QuoteForEnqueryId));
                modal.IsQuoteFullfillRequirement = repoNewPlanMyHoliday.NewPackageRequestStatusId == (int)PackageRequestStatus.Completed ? true : false;
                modal.IsItRequiredMoreChanges = repoNewPlanMyHoliday.NewPackageRequestStatusId == (int)PackageRequestStatus.Completed ? false : true;
                modal.IsQuoteFullfillRequirementVal = repoNewPlanMyHoliday.NewPackageRequestStatusId == (int)PackageRequestStatus.Completed ? "1" : "0";
                modal.IsItRequiredMoreChangesVal = repoNewPlanMyHoliday.NewPackageRequestStatusId == (int)PackageRequestStatus.Completed ? "0" : "1";

                //modal.IsQuoteApproved = false;
            }
            var userobj = _repoUser.GetUserById(Convert.ToInt32(uid));

            modal.QuoteForEnqueryId = newPKG.QuoteForEnqueryId;
            modal.FeedbackForEnqueryTypeName = newPKG.QuoteForEnqueryType == 0 ? "Customize Package" : "New Package";
            modal.QuoteVersion = newPKG.QuoteVersion;
            modal.QuoteStatusId = newPKG.QuoteStatusId;
            modal.ParentPackageId = newPKG.ParentPackageId;

            modal.Email = userobj.Email;
            modal.UserId = userobj.Id;
            modal.PhoneNumber = userobj.MobilePhone;
            modal.UserName = userobj.FirstName + " " + userobj.LastName;




            var newReq1 = _quoteFeedbackService.GetQuoteFeedbackByPackageIdAndEnqueryTypeList(Convert.ToInt64(pkgid), typ).Where(x => x.IsActive == true && x.IsDeleted == false).OrderBy(x => x.Id).ToList();
            if (newReq1.Count > 0)
            {
                var newReq = newReq1.SingleOrDefault();
                if (newReq != null)
                {
                    modalchild.Id = newReq.Id;
                    modalchild.PackageId = newReq.PackageId;

                    modalchild.IsActive = newReq.IsActive == true ? "Yes" : "No";
                    modalchild.CreatedOn = newReq.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime);
                    modalchild.PackageUrlForQuote = newReq.PackageUrlForQuote;
                    modalchild.FeedbackForEnqueryType = newReq.FeedbackForEnqueryType;
                    modalchild.EmailContentForQuote = newReq.EmailContentForQuote;
                    modal.LastQuoteFeedbackForModel = modalchild;

                    //parent model property assignment
                    //modal.EmailContentForQuote = newReq.EmailContentForQuote;
                    modal.EnqueryTypeForQuote = newReq.FeedbackForEnqueryType;

                    //modal.PreviousFeedbackDescriptionForQuote = newReq.FeedbackDescription;
                    modal.PreviousEmailContentForQuote = newReq.EmailContentForQuote;
                    //modal.FeedbackDescription = "";
                    modal.EmailContentForQuote= "";

                    modal.PackageId = newReq.PackageId;
                    modal.PackageUrlForQuote = newReq.PackageUrlForQuote;
                    //modal.EmailContentForQuote = newReq.EmailContentForQuote;
                }
            }
            else
            {
                QuoteFeedbackForModel m = new QuoteFeedbackForModel();
                modal.LastQuoteFeedbackForModel = m;
            }
            return View(modal);
        }
        // verify user existens and set active


        [HttpPost]
        public async Task<IActionResult> QuoteFeedbackSubmit([FromForm] QuoteFeedbackViewModel model)
        {
            try
            {
                Data.Models.QuoteFeedback quotefeedback = null;
                if (model.LastQuoteFeedbackForModel.Id == 0)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Invalid request", IsSuccess = false });
                }

                if (model.IsItRequiredMoreChangesVal == "1")
                {
                    if (string.IsNullOrEmpty(model.EmailContentForQuote))
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Email Content (Feedback message) required.", IsSuccess = false });
                    }
                }

                quotefeedback = _quoteFeedbackService.GetQuoteFeedbackByPackageIdAndEnqueryTypeList(Convert.ToInt64(model.PackageId), Convert.ToInt32(model.EnqueryTypeForQuote)).Where(x => x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                if (quotefeedback != null)
                {
                    quotefeedback.PackageId = model.PackageId;
                    quotefeedback.FeedbackForEnqueryType = model.EnqueryTypeForQuote;
                    //quotefeedback.FeedbackDescription = model.FeedbackDescription;
                    quotefeedback.IsActive = true;
                    quotefeedback.IsDeleted = false;
                    quotefeedback.CreatedBy = model.PackageId;
                    quotefeedback.CreatedOn = DateTime.UtcNow;
                    quotefeedback.QuteFeedbackStatus = 0;


                    if (model.PreviousEmailContentForQuote== null || model.PreviousEmailContentForQuote == "")
                    {
                        quotefeedback.EmailContentForQuote = "<div class='msguser'><div class='msg'>" + model.EmailContentForQuote + "</div><div class='msgdate'>Date: " + DateTime.UtcNow + "</div></div>";
                    }
                    else
                    {
                        quotefeedback.EmailContentForQuote = model.PreviousEmailContentForQuote + "<div class='msguser'><div class='msg'>" + model.EmailContentForQuote + "</div><div class='msgdate'>Date: " + DateTime.UtcNow + "</div></div>";
                    }

                    quotefeedback = await _quoteFeedbackService.UpdateQuoteFeedback(quotefeedback);


                    NewPackageRequest repoNewPlanMyHoliday = null;
                    CustomizePackageRequest repoNewCustomQuote = null;
                    if (model.EnqueryTypeForQuote == 0)
                    {
                        int cprid = Convert.ToInt32(model.QuoteForEnqueryId);
                        repoNewCustomQuote = _customQuoteService.GetCustomizePackageRequestById(cprid);
                        repoNewCustomQuote.PackageRequestStatusId = (model.IsQuoteFullfillRequirementVal != "" && model.IsQuoteFullfillRequirementVal == "1") ? (int)PackageRequestStatus.Completed : (int)PackageRequestStatus.InProgress;
                        await _customQuoteService.UpdateCustomizePackageRequestasync(repoNewCustomQuote);
                    }
                    else
                    {
                        repoNewPlanMyHoliday = _repoNewPkgReq.GetNewPackageRequestById(Convert.ToInt32(model.QuoteForEnqueryId));
                        repoNewPlanMyHoliday.NewPackageRequestStatusId = (model.IsQuoteFullfillRequirementVal != "" && model.IsQuoteFullfillRequirementVal == "1") ? (int)PackageRequestStatus.Completed : (int)PackageRequestStatus.InProgress;
                        await _repoNewPkgReq.UpdateNewPackageRequest(repoNewPlanMyHoliday);
                    }
                }
                else
                {

                    quotefeedback = new QuoteFeedback();

                    quotefeedback.PackageId = model.PackageId;
                    quotefeedback.FeedbackForEnqueryType = model.EnqueryTypeForQuote;
                    //quotefeedback.FeedbackDescription = model.FeedbackDescription;
                    if (model.PreviousEmailContentForQuote == null || model.PreviousEmailContentForQuote == "")
                    {
                        quotefeedback.EmailContentForQuote = "<div class='msguser'><div class='msg'>" + model.EmailContentForQuote + "</div><div class='msgdate'>Date: " + DateTime.UtcNow + "</div></div>";
                    }
                    else
                    {
                        quotefeedback.EmailContentForQuote = model.PreviousEmailContentForQuote + "<div class='msguser'><div class='msg'>" + model.EmailContentForQuote + "</div><div class='msgdate'>Date: " + DateTime.UtcNow + "</div></div>";
                    }

                    quotefeedback.IsActive = true;
                    quotefeedback.IsDeleted = false;
                    quotefeedback.CreatedBy = model.PackageId;
                    quotefeedback.CreatedOn = DateTime.UtcNow;
                    quotefeedback.QuteFeedbackStatus = 0;



                    quotefeedback = await _quoteFeedbackService.SaveQuoteFeedback(quotefeedback);


                    NewPackageRequest repoNewPlanMyHoliday = null;
                    CustomizePackageRequest repoNewCustomQuote = null;
                    if (model.EnqueryTypeForQuote == 0)
                    {
                        int cprid = Convert.ToInt32(model.QuoteForEnqueryId);
                        repoNewCustomQuote = _customQuoteService.GetCustomizePackageRequestById(cprid);
                        repoNewCustomQuote.PackageRequestStatusId = (model.IsQuoteFullfillRequirementVal != "" && model.IsQuoteFullfillRequirementVal == "1") ? (int)PackageRequestStatus.Completed : (int)PackageRequestStatus.InProgress;
                        await _customQuoteService.UpdateCustomizePackageRequestasync(repoNewCustomQuote);
                    }
                    else
                    {
                        repoNewPlanMyHoliday = _repoNewPkgReq.GetNewPackageRequestById(Convert.ToInt32(model.QuoteForEnqueryId));
                        repoNewPlanMyHoliday.NewPackageRequestStatusId = (model.IsQuoteFullfillRequirementVal != "" && model.IsQuoteFullfillRequirementVal == "1") ? (int)PackageRequestStatus.Completed : (int)PackageRequestStatus.InProgress;
                        await _repoNewPkgReq.UpdateNewPackageRequest(repoNewPlanMyHoliday);
                    }
                }



                var newPKG = _packageService.GetActivePackageById(Convert.ToInt64(model.PackageId));
                if (newPKG != null && !newPKG.IsDeleted && newPKG.IsActive)
                {
                    newPKG.IsQuoteVerified = true;
                    newPKG.QuoteVerificationOtpExpired = null;
                    await _packageService.Save(newPKG);
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Package either deactivated or removed, please contact to admin.", IsSuccess = true });
                }

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Feedback has been submitted successfully", IsSuccess = true });
            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = e.Message, IsSuccess = false });

            }
        }

        #endregion

        [HttpPost]
        public IActionResult CheckUserStatus()
        {
            try
            {
                var Idd = 0;
                Idd = Convert.ToInt32(ContextProvider.HttpContext.User.Claims.FirstOrDefault(u => u.Type == nameof(Id))?.Value);
                var user = _userService.GetUserById(Idd);
                //var user = _adminUserService.GetAdminUserById(Idd);
                bool ststus = false;
                if (user != null)
                {
                    ststus = Convert.ToBoolean(user.IsActive);
                    if(ststus==false)
                    {
                        SignOut();
                    }
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Data = ststus, IsSuccess = ststus == true ? true : false });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Data = ststus, IsSuccess = true });
                }

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Data = null, IsSuccess = false });


            }

        }
        public int Id { get; private set; }
    }
}
