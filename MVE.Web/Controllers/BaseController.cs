using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TCP.Data.Models;
using TCP.Core.Models.Security;
using Newtonsoft.Json;
using TCP.Core.Code.LIBS;
using TCP.Core.Models.Others;
using System.Globalization;
using TCP.Dto;

namespace TCP.Web.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class BaseController : Controller
    {

        public BaseController()
        {
        }


        #region [ CLAIM PROPERTIES ]

        public CustomPrincipal CurrentFrontUser => new CustomPrincipal(HttpContext.User);

        #endregion [ CLAIM PROPERTIES ]

        #region [ CREATE AUTHENTICATION ]

        /// <summary>
        /// Create A Ticket For Authenticate User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>return Claim Properties  With Filled Values </returns>
        //public async Task CreateAuthenticationTicket(AdminUser user, int[] userPermissionIds, string roleType, bool remember)
        //{
        //    if (user != null)
        //    {
        //        //var permissions = user.UserPermission.Select(x => x.UserPermissionId).Distinct().ToArray();

        //        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        //        identity.AddClaim(new Claim(ClaimTypes.PrimarySid, Convert.ToString(user.Id)));
        //        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        //        identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName));
        //        identity.AddClaim(new Claim(ClaimTypes.Role, roleType));
        //        identity.AddClaim(new Claim(nameof(user.Id), Convert.ToString(user.Id)));
        //        identity.AddClaim(new Claim(nameof(user.Email), user.Email));
        //        identity.AddClaim(new Claim(nameof(user.FirstName), user.FirstName));
        //        identity.AddClaim(new Claim(nameof(user.LastName), user.LastName));
        //        identity.AddClaim(new Claim(nameof(user.ImageName), user.ImageName ?? string.Empty));
        //        identity.AddClaim(new Claim(nameof(user.IsActive), Convert.ToString(user.IsActive)));

        //        identity.AddClaim(new Claim("userPermissions", JsonConvert.SerializeObject(userPermissionIds)));

        //        var props = new AuthenticationProperties();
        //        props.IsPersistent = remember;

        //        var principal = new ClaimsPrincipal(identity);
        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);


        //    }
        //}
        public async Task CreateAuthenticationTicketUser(User user, bool remember)
        {
            if (user != null)
            {
                
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.PrimarySid, Convert.ToString(user.Id)));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.FirstName??""));
                //identity.AddClaim(new Claim(ClaimTypes.Role, roleType));
                identity.AddClaim(new Claim(nameof(user.Id), Convert.ToString(user.Id)));
                identity.AddClaim(new Claim(nameof(user.Email), user.Email));
                identity.AddClaim(new Claim(nameof(user.FirstName), user.FirstName??""));
                identity.AddClaim(new Claim(nameof(user.LastName), user.LastName??""));
                identity.AddClaim(new Claim(nameof(user.ImageName), user.ImageName ?? string.Empty));
                identity.AddClaim(new Claim(nameof(user.IsActive), Convert.ToString(user.IsActive)));

                //var siteSetting = JsonConvert.SerializeObject(GlobalSetting);
                //identity.AddClaim(new Claim("SiteSettings", siteSetting));

                //  identity.AddClaim(new Claim("userPermissions", JsonConvert.SerializeObject(userPermissionIds)));

                var props = new AuthenticationProperties();
                props.IsPersistent = remember;

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);


            }
        }


        /// <summary>
        /// remove Authentication From Cookies..
        /// </summary>
        /// <returns>return true after set null </returns>
        public async Task RemoveAuthentication()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        #endregion [ CREATE AUTHENTICATION ]

        #region [ MESSAGE NOTIFICATION ]

        /// <summary>
        /// Shows Notification Message After Succesfully Record Submited or Failed to Submit any Record From Submit Of Form.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="isCurrentView"></param>
        private void ShowMessages(string title, string message, MessageType messageType, bool isCurrentView)
        {
            Notification model = new Notification
            {
                Heading = title,
                Message = message,
                Type = messageType
            };

            if (isCurrentView)
            {
                ViewData.AddOrReplace("NotificationModel", model);
            }
            else
            {
                TempData["NotificationModel"] = JsonConvert.SerializeObject(model);
                TempData.Keep("NotificationModel");
            }
        }

        protected void ShowErrorMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Danger, isCurrentView);
        }

        protected void ShowSuccessMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Success, isCurrentView);
        }

        protected void ShowWarningMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Warning, isCurrentView);
        }

        protected void ShowInfoMessage(string title, string message, bool isCurrentView = true)
        {
            ShowMessages(title, message, MessageType.Info, isCurrentView);
        }


        #endregion [ MESSAGE NOTIFICATION ]

        #region [ HTTP ERROR REDIRECTION ]

        /// <summary>
        /// redirect to respected Status Code of Error
        /// </summary>
        /// <returns>redirect to 404</returns>
        protected ActionResult Redirect404()
        {
            return Redirect("~/error/pagenotfound");
        }

        /// <summary>
        /// redirect to respected Status Code of Error
        /// </summary>
        /// <returns>redirect to 500</returns>
        protected ActionResult Redirect500()
        {
            return Redirect("~/error/servererror");
        }

        protected ActionResult Redirect401()
        {
            return View();
        }

        #endregion [ HTTP ERROR REDIRECTION ]

        #region [ EXCEPTION HANDLING ]
        /// <summary>
        /// Handle Exception Of Model State Of Validation Summary
        /// </summary>
        /// <returns>return Error Message</returns>
        public PartialViewResult CreateModelStateErrors()
        {
            return PartialView("_ValidationSummary", ModelState.Values.SelectMany(x => x.Errors));
        }

        #endregion [ EXCEPTION HANDLING ]

        #region  [ SERIALIZATION ]
        /// <summary>
        /// Serilize Data into Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns>return Json Data</returns>
        public IActionResult NewtonSoftJsonResult(object data)
        {
            return Json(data);
        }

        //public ActionResult NewtonSoftJsonResult(object data)
        //{
        //    //Override IIS default status code message structure
        //    Response.TrySkipIisCustomErrors = true;
        //    return new JsonNetResult
        //    {
        //        Data = data,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //        MaxJsonLength = Int32.MaxValue
        //        //Use this value to set your maximum size for all of your Requests
        //    };
        //}
        #endregion [ SERIALIZATION ]

        #region [ DISPOSE ]

        /// <summary>
        /// Dispose All Service 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion [ DISPOSE ]

        public string RequestIPAddress(IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        protected int? GetCurrentUserId(IHttpContextAccessor accessor)
        {
            string userId = accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrWhiteSpace(userId) ? Convert.ToInt32(userId) : (int?)null;
        }
        //protected void ComposeUserTracking<T>(T model, IHttpContextAccessor accessor) where T : class, IUserTracking
        //{
        //    //var output = new T();
        //    model.IpAddress = this.RequestIPAddress(accessor);
        //    model.UpdatedBy = CurrentUser.Id;
        //}

        protected string GeneratePassword()
        {
            int length = 6;

            bool nonAlphanumeric = true;
            bool digit = true;
            bool lowercase = true;
            bool uppercase = true;

            System.Text.StringBuilder password = new System.Text.StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString();
        }

        //public static DateTime ParsestringDateintoDatetime(string date)
        //{
        //    CultureInfo provider = new CultureInfo("en-US");
        //    DateTime date1 = DateTime.Parse(date, provider, DateTimeStyles.AdjustToUniversal);

        //    return date1;
        //}

        public static DateTime ParsestringDateintoDatetime1(string date)
        {
            // "yyyy-MM-dd"
            //string date1 = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //return DateTime.ParseExact(date1.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dt = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);           
            return dt;
        }

    }
}
