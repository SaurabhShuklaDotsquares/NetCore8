using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Text.Json;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.LIBS;
using MVE.Core.Models;
using MVE.Data.Models;
using MVE.Dto;
using MVE.Service;
using System;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Security.Claims;
using MVE.Core.Code.Attributes;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using UAParser;
using OpenQA.Selenium.Chrome;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http.Extensions;

namespace MVE.Admin.Controllers
{
    //[Route("Admin")]
    public class AccountController : BaseController
    {
        private readonly IConfiguration _configuration;
        private IDataProtector _protector;
        private readonly IAdminUserService _adminUserService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly IPermissionService _permissionService;
        private readonly IUserRoleService _userRoleService;

        private readonly ISettingService _settingService;
        public AccountController(IConfiguration configuration, IDataProtectionProvider provider, IAdminUserService adminUserService, IEmailFactoryService emailFactoryService
            , IPermissionService permissionService, IUserRoleService userRoleService, ISettingService settingService)
        {
            _configuration = configuration;
            var protectorPurpose = "secure username and password";
            _protector = provider.CreateProtector(protectorPurpose);
            _adminUserService = adminUserService;
            _emailFactoryService = emailFactoryService;
            _permissionService = permissionService;
            _userRoleService = userRoleService;
            _settingService = settingService;
        }


        #region [LogIn]
        public async Task<IActionResult> Index()
        {
            if (CurrentUser.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard"); //,new { Area = "Admin" }
            }
            var model = new LoginViewModel();
            //var userKey = Request.Cookies["UserKey"];
            // var userPwd = Request.Cookies["UserValue"];
            var userKey = Request.Cookies["myUser"];
            var userPwd = Request.Cookies["myKey"];
            if (!string.IsNullOrEmpty(userKey) && !string.IsNullOrEmpty(userPwd))
            {
                string unProtName = string.Empty;
                string unProtPwd = string.Empty;
                try
                {
                    //unProtName = _protector.Unprotect(userKey);
                    //unProtPwd = _protector.Unprotect(userPwd);
                    unProtName = userKey;
                    unProtPwd = userPwd;
                    if (!string.IsNullOrEmpty(unProtName) && !string.IsNullOrEmpty(unProtPwd))
                    {
                        model.Email = unProtName;
                        model.Password = unProtPwd;
                        if (!string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Email))
                        {
                            model.RememberMe = true;
                            //await SignOut();
                            //var isRedirect = await loginAsync(model);
                            //if (string.IsNullOrEmpty(isRedirect))
                            //{
                            //    return RedirectToAction("Index", "Dashboard");
                            //}
                            //else
                            //{
                            //    return View(model);
                            //}
                            return View(model);
                        }
                    }
                }
                catch (Exception)
                {
                    Response.Cookies.Delete("UserKey");
                    Response.Cookies.Delete("UserValue");
                }
            }
            else
            {
                await SignOut();
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            var validMessage = await loginAsync(model);
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = validMessage, RedirectUrl = @Url.Action("Index", "Dashboard") });
        }
        private async Task<string> loginAsync(LoginViewModel model)
        {
            //var userAgent = HttpContext.Request.Headers["User-Agent"];
            //var uaParser = Parser.GetDefault();
            //ClientInfo c = uaParser.Parse(userAgent);
            ////*****
            //// Edge WebDriver
            //if(c.UserAgent.Family.ToLower()== "edge")
            //{
            //    var edgeOptions = new EdgeOptions();

            //    // Set headless mode to hide the browser window
            //    edgeOptions.AddArgument("headless");

            //    // Disable opening a new window
            //    edgeOptions.AddAdditionalEdgeOption("useAutomationExtension", false);

            //    var edgeDriver = new EdgeDriver(edgeOptions);

            //    edgeDriver.Navigate().GoToUrl(SiteKeys.Domain);

            //    IWebElement edgeRememberMeCheckbox = edgeDriver.FindElement(By.Id("RememberMe")); // Replace with your checkbox ID

            //    bool isEdgeSelected = edgeRememberMeCheckbox.Selected;

            //    edgeDriver.Quit();

            //}
            
            string displayMsg = string.Empty;
            try
            {
                model.Email = model.Email.Trim();
                model.Password = model.Password.Trim();
                var user = _adminUserService.GetAdminUserByEmail(model.Email);
                if (ModelState.IsValid)
                {
                    if (user != null)
                    {
                        if (!user.IsDeleted)
                        {
                            if ((bool)user.IsActive)
                            {
                                var match = false;
                                match = PasswordEncryption.IsPasswordMatch(user.EncryptedPassword, model.Password, user.SaltKey);
                                if (match)
                                {
                                    var roleId = user.RoleId;
                                    var roleType = user.Role.RoleName;
                                    //var userPermissionIds = _permissionService.GetPermissionByRoleId(roleId);
                                    //var userPermissionIds = new int[] { 0 };
                                    var GlobalSetting = GetGlobalSetting();

                                    var userPermissionIds = _userRoleService.GetPermissionByRoleId(roleId);
                                    var allActionPermissions = GetAllActionPermissionsOfRole(roleId);
                                    await CreateAuthenticationTicket(user, userPermissionIds, roleType, model.RememberMe, allActionPermissions, GlobalSetting);
                                    var cookieOptions = new CookieOptions
                                    {
                                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                                        IsEssential = true,
                                        SameSite = SameSiteMode.Lax,
                                        HttpOnly = false,
                                        Domain = SiteKeys.Domain,
                                        Path = "/admin",
                                        Secure = true, // if served over HTTPS
                                    };
                                    if (model.RememberMe)
                                    {
                                        var protectedName = _protector.Protect(model.Email.Trim());
                                        var protectedPassword = _protector.Protect(model.Password);
                                        Response.Cookies.Append("UserKey", protectedName, cookieOptions);
                                        Response.Cookies.Append("UserValue", protectedPassword, cookieOptions);

                                        //HttpContext.Session.SetString("UserName", model.Email);
                                        //HttpContext.Session.SetString("Password", model.Password);

                                        Response.Cookies.Append("myUser", model.Email, cookieOptions);
                                        Response.Cookies.Append("myKey", model.Password, cookieOptions);

                                       
                                    }
                                    else
                                    {
                                        Response.Cookies.Delete("UserKey");
                                        Response.Cookies.Delete("UserValue");

                                        Response.Cookies.Append("myUser", "", cookieOptions);
                                        Response.Cookies.Append("myKey", "", cookieOptions);

                                    }
                                    List<MenuItem> menuItems = new List<MenuItem>();
                                    List<MenuViewModel> menuItemsList = new List<MenuViewModel>();
                                    menuItems = _permissionService.GetAllMenus();

                                    MenuViewModel.MenuItems = menuItems.Select(item => new MenuViewModel
                                    {
                                        MenuName = item.Name,
                                        SubMenuName = item.SubMenuName,
                                        MenuIcon = item.Icon,
                                        SubMenuIcon = item.SubMenuIcon,
                                        MenuUrl = item.Url,
                                        IsActive = item.IsActive.Value,
                                        PermissionId = item.PermissionId.Value
                                    }).ToList();

                                    displayMsg = string.Empty;
                                    //SetAllLanguageInTempView();
                                }
                                else
                                {
                                    displayMsg = "Password is incorrect.";
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
                obj.AdminPageLimit = globalSetting.AdminPageLimit;
                obj.SupportEmail = globalSetting.SupportEmail;
                obj.SupportMobile = globalSetting.SupportMobile;
                obj.LogoImageName = globalSetting.LogoImageName;
                obj.LogoImageNameDark = globalSetting.LogoImageNameDark;
            }
            else
            {
                int pageSize = 10;
                obj.AdminPageLimit = int.TryParse(SiteKeys.AdminPageLimit, out pageSize) ? Convert.ToInt16(SiteKeys.AdminPageLimit) : pageSize;
                obj.SupportEmail = SiteKeys.SupportEmail;
                obj.SupportMobile = SiteKeys.SupportMobile;
                obj.LogoImageName = SiteKeys.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.LogoImageNameDark;
            }
            return obj;
        }

        private List<RoleActionPermissionDTO> GetAllActionPermissionsOfRole(long roleId)
        {
            List<RoleActionPermissionDTO> per = new List<RoleActionPermissionDTO>();
            try
            {
                var allRolePages = _userRoleService.GetRolePagePermissionsByRoleId(roleId);
                if (allRolePages.Count() > 0)
                {
                    foreach (var item in allRolePages)
                    {
                        per.Add(new RoleActionPermissionDTO
                        {
                            RoleId = item.RoleId,
                            PageId = item.PageId,
                            IsReadOnly = item.IsReadOnly,
                            IsCreate = item.IsCreate,
                            IsEdit = item.IsEdit,
                            IsDelete = item.IsDelete
                        });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return per;
        }
        #endregion

        #region Sign Out
        /// <summary>
        /// Represents an event that is raised when the sign-out operation is complete.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SignOut()
        {
            await signOutFunctionality();
            Response.Cookies.Delete("UserKey");
            Response.Cookies.Delete("UserValue");
            return RedirectToAction("index");
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
        [Route("[Controller]/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _adminUserService.GetAdminUserByEmail(model.Email);
                    if (user != null)
                    {
                        if (!user.IsDeleted)
                        {
                            user.ForgotPasswordLink = Guid.NewGuid().ToString();
                            user.ForgotPasswordLinkExpired = DateTime.Now.AddDays(30);
                            user.ForgotPasswordLinkUsed = false;
                            user.ModifiedOn = DateTime.Now;
                            await _adminUserService.UpdateAdminUserAsync(user);
                            if (await _emailFactoryService.SendAdminUserForgotPasswordEmail(user, model.Email))
                            {
                                ShowSuccessMessage("Success", "Reset password link has been sent successfully to your registered Email address.", false);
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {
                            ShowErrorMessage("Error!", "Please provide valid Email address.", false);
                            return RedirectToAction("ForgotPassword");
                        }
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Please provide valid Email address.", false);
                        return RedirectToAction("ForgotPassword");
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", "Didn't find any email address to send the reset password link", false);
                    return RedirectToAction("ForgotPassword");
                }

            }
            catch
            {
                ShowErrorMessage("Error!", "Something went wrong with the process", false);
                return RedirectToAction("ForgotPassword");
            }

            ModelState.Clear();
            model.Email = string.Empty;
            return View(model);
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public IActionResult ResetPassword(string id)
        {
            try
            {
                var user = _adminUserService.GetAdminUserByGuid(id);
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
                    var adminUser = _adminUserService.GetAdminUserByGuid(model.Id);
                    if (adminUser != null)
                    {
                        if (!adminUser.IsDeleted)
                        {
                            adminUser.ForgotPasswordLink = null;
                            adminUser.ForgotPasswordLinkExpired = null;
                            adminUser.ForgotPasswordLinkUsed = true;
                            adminUser.SaltKey = PasswordEncryption.CreateSaltKey();
                            adminUser.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.Password, adminUser.SaltKey);
                            await _adminUserService.UpdateAdminUserAsync(adminUser);
                            ShowSuccessMessage("Success", "Your password has been reset successfully, please login to continue", false);
                            return RedirectToAction("index");
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

            return RedirectToAction("index");
        }
        #endregion

        #region [ LANGUAGE ]

        public void SetAllLanguageInTempView()
        {
            List<LanguageDTO> allLanguages = new List<LanguageDTO>();
            allLanguages.Add(new LanguageDTO
            {
                Language = "English",
                LanguageId = 1,
                IsDefault = true
            });
            TempData["Languages"] = JsonSerializer.Serialize(allLanguages);
            TempData.Keep("Languages");
        }

        #endregion


        [HttpPost]
        public IActionResult CheckUserStatus()
        {
            try
            {
                var Idd = 0;
                Idd = Convert.ToInt32(ContextProvider.HttpContext.User.Claims.FirstOrDefault(u => u.Type == nameof(Id))?.Value);

                var user = _adminUserService.GetAdminUserById(Idd);
                bool ststus = false;
                if (user != null)
                {
                    ststus = Convert.ToBoolean(user.IsActive);

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
