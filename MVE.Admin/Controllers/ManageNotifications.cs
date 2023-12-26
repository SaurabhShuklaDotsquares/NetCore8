using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text.RegularExpressions;
using MVE.Admin.ViewModels;
using MVE.Admin.ViewModels.Notifications;
using MVE.Core;
using MVE.Core.Code.Attributes;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Others;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class ManageNotifications : BaseController
    {
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly IEmailFactoryService _emailFactoryService;
        public ManageNotifications(INotificationService notificationService, IUserService userService, IEmailFactoryService emailFactoryService)
        {
            _userService = userService;
            _notificationService = notificationService;
            _emailFactoryService = emailFactoryService;
        }
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageNotifications, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {
            ManageNotificationViewModel notificationViewModel = new ManageNotificationViewModel();
            ViewBag.Notification = _notificationService.GetAllNotificationType().Select(x => new SelectListItem { Text = x.Name.ToString(), Value = x.Id.ToString() }).ToList();
            return View(notificationViewModel);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageNotifications, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public ActionResult SendNotification()
        {
            ManageNotificationViewModel notificationViewModel = new ManageNotificationViewModel();
            ViewBag.Allusers = _userService.GetAllUserNotDeleted().Where(u => !string.IsNullOrEmpty(u.FirstName)&&u.IsActive).Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            //ViewBag.Allusers = _userService.GetAllUserNotDeleted().Where(u => u.FirstName != null && u.IsEmailVerified != false).Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            return View(notificationViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> SendNotification(ManageNotificationViewModel notificationViewModel)
        {
            try
            {
                if (notificationViewModel.SelectedUsers != null)
                {
                    if (notificationViewModel.Description != null)
                    {
                        if (notificationViewModel.Description.Length <= 500)
                        {
                            FileInfo fileinfo = new FileInfo(notificationViewModel?.FlagImage?.FileName);
                            foreach (var user in notificationViewModel.SelectedUsers)
                            {
                                var model = _userService.GetUserById(Convert.ToInt64(user));

                                var userNotification = new UserNotification
                                {
                                    UserId = model?.Id,
                                    ImageName = notificationViewModel?.FlagImage?.FileName,
                                    Title = notificationViewModel?.Title,
                                    Descriptions = notificationViewModel?.Description,
                                    NotificationTypeId = 7,
                                    SentType = 1,
                                    IsActive = true,
                                    CreatedDate = DateTime.Now,
                                    CreatedBy = CurrentUser.Id,
                                    ImageExtension = fileinfo.Extension,
                                    Subject = "Notification"
                                };
                                await _notificationService.SaveUserNotification(userNotification);
                                if (model.Email != null && userNotification.Id != 0)
                                {
                                    if (notificationViewModel.FlagImage?.FileName != null)
                                    {
                                        CommonFileViewModel.FileUpload(notificationViewModel.FlagImage, SiteKeys.UploadFilesNotifications);
                                    }
                                    string userName = model.FirstName + " " + model.LastName;
                                    await Task.Run(() => { _emailFactoryService.SendNotification(userNotification, userName, model.Email); });
                                    string displayMsg = $"Notification has been sent successfully.";
                                    ModelState.Clear();
                                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
                                }
                            }
                        }
                        else
                        {
                            return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Notifications Description should be less than 500 characters", IsSuccess = false });
                        }
                    }
                    else
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<string> { Message = "Notifications Description is required", IsSuccess = false });
                    }
                }     
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "User is required", IsSuccess = false });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = ex.Message, IsSuccess = false });
            }

        }


        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable, string notificationType)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<UserNotification>();
            if (notificationType != null)
            {
                query.AddFilter(b => b.NotificationTypeId == Convert.ToInt16(notificationType));
            }
            query.AddFilter(q => q.IsDeleted == false);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {

                string sSearch = dataTable.sSearch.Trim().ToLower();
                var allNotificationType = _notificationService.GetAllNotificationType().Where(x => x.Name.ToLower().Trim().Contains(sSearch.ToLower().Trim())).Select(x => x.Id).ToList();
                query.AddFilter(q => q.Title.Contains(sSearch) || (q.Descriptions ?? string.Empty).Contains(sSearch) || allNotificationType.Contains(q.NotificationTypeId));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
               
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserNotification, int>(q => q.NotificationTypeId, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserNotification, DateTime>(q => q.CreatedDate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserNotification, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;

                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserNotification, DateTime>(q => q.CreatedDate, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;

            IEnumerable<UserNotification> notificationsobj = _notificationService.Get(query, out total).Entities;

            foreach (UserNotification r in notificationsobj)
            {
                var notificationsTypeName = _notificationService.GetAllNotificationType().Where(x => x.Id == r.NotificationTypeId).Select(s => s.Name).FirstOrDefault();
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                     count.ToString(),
                     r.ImageName??"",
                     r.Title,
                     notificationsTypeName,
                    r.CreatedDate.ToString(SiteKeys.DateFormatWithoutTime),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageNotifications, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult ViewNotification(int? id)
        {
            ManageNotificationViewModel model = new ManageNotificationViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _notificationService.GetUserNotificationById(id.Value);
                if (accObj != null)
                {
                   
                    var userModel = _userService.GetUserById(Convert.ToInt64(accObj.UserId));
                    if (userModel != null)
                    {
                        model.Id = Convert.ToInt16(userModel.Id);
                        model.Name = userModel.FirstName + " " + userModel.LastName;
                        model.Email = userModel.Email;
                        model.MobilePhone = userModel.MobilePhone;
                        model.Title = accObj.Title;
                        string descriptionWithoutHtml = Regex.Replace(accObj.Descriptions, "<.*?>", string.Empty);
                        model.Description = descriptionWithoutHtml;
                        model.FileName = (string.IsNullOrEmpty(accObj.ImageName) ? "" : SiteKeys.UploadFilesNotifications + accObj.ImageName);
                        model.FlagImage = new FormFile(null, 0, 0, SiteKeys.UploadFilesCountry + accObj.ImageName, accObj.ImageName);
                    }

                     accObj.IsVisited = true;
                    _notificationService.UpdateUserNotification(accObj);

                }
            }
            return PartialView("_ViewNotification", model);
        }




        [HttpPost]
        public IActionResult GetUnvisitedNotification()
        {
            try
            {
                var data = _notificationService.GetAllUnvisitedUserNotification();


                int count = data.Count;
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Data = count, IsSuccess = true });
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Data = null, IsSuccess = false });


            }

        }


    }
}
