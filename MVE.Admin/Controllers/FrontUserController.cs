using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVE.Core.Code.LIBS;
using MVE.Core;
using MVE.Admin.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using MVE.Service;
using MVE.DataTable.Extension;
using MVE.Core.Code.Attributes;
using System.Globalization;
using System.Linq.Expressions;
using MVE.Core.Models.Others;
using System.Data;
using ClosedXML.Excel;
using Castle.Core.Resource;
using MVE.Data.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;
using MVE.Core.Code.Notification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class FrontUserController : BaseController
    {
        #region [ SERVICE INJECTION ]
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFrontUserService _frontUserService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly INotificationService _notificationService;

        private readonly IUserService _userService;

        public FrontUserController(INotificationService notificationService, IConfiguration configuration, IHostingEnvironment hostingEnvironment, IFrontUserService frontUserService,  IEmailFactoryService emailFactoryService, IUserService userService)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _frontUserService = frontUserService;
            _emailFactoryService = emailFactoryService;
            _notificationService = notificationService;
            _userService = userService;
        }

        #endregion [ SERVICE INJECTION ]

        #region [ INDEX ]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsRead)]
        [HttpGet]
        [Route("[Controller]/[Action]")]
        public IActionResult Index()
        {
            FrontUserViewModel model = new FrontUserViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(MVE.DataTable.DataTables.DataTable request, bool? status, string? fStartDate, string? fEndDate, string txtSearchFilter)
        {
            FrontUserViewModel vm = new FrontUserViewModel(request);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];

            //Get filter query
            var query = vm.FilterGrid(status, sortColumnIndex, sortDirection);


            if (!string.IsNullOrEmpty(txtSearchFilter))
            {
                string sSearch = txtSearchFilter.ToLower().Trim().TrimStart().TrimEnd();
                var name_search = sSearch.Split(' ');
                query.AddFilter(ad => (ad.FirstName.Trim() + " " + ad.LastName.Trim()).Contains(sSearch) ||
                 ad.Email.ToLower().Trim().Contains(sSearch.Trim()) ||
                 ad.Id.ToString().Contains(sSearch) ||
                 ad.MobilePhone.ToLower().Trim().Contains(sSearch) || ad.Address.ToLower().Trim().Contains(sSearch)
                 );
            }
            if (!string.IsNullOrEmpty(fStartDate) && !string.IsNullOrEmpty(fEndDate))
            {
                DateTime sDate = DateTime.ParseExact(fStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime eDate = DateTime.ParseExact(fEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                query.AddFilter(ad => ad.CreationOn >= sDate && ad.CreationOn <= eDate.AddDays(1).Date);
            }
            else if (!string.IsNullOrEmpty(fStartDate))
            {
                DateTime sDate = DateTime.ParseExact(fStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                query.AddFilter(ad => ad.CreationOn >= sDate);
            }
            else if (!string.IsNullOrEmpty(fEndDate))
            {
                DateTime eDate = DateTime.ParseExact(fEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                query.AddFilter(ad => ad.CreationOn.Date <= eDate);
            }

            //get data
            var frontUsers = _frontUserService.Search(query, out int totalCount).Entities;

            vm.SetEntity(frontUsers);

            vm.ComposeViewData();

            return new DataTableResultExt(request, vm.GridViewData.Count(), totalCount, vm.GridViewData);
        }

        #endregion [ INDEX ]
        #region [View Details]
        [HttpGet]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsRead)]
        public IActionResult ViewFrontUser(int? id)
        {
            FrontUserViewModel modal = new FrontUserViewModel();
            modal.Id = id ?? 0;
            try
            {
                var newReq = _frontUserService.GetFrontUserById(modal.Id);
                if (newReq != null)
                {
                    modal.Id = newReq.Id;
                    modal.CreationOn = newReq.CreationOn;
                    modal.FirstName = newReq.FirstName;
                    modal.LastName = newReq.LastName;
                    modal.Email = newReq.Email;
                    modal.MobilePhone = newReq.MobilePhone;

                    modal.ImageName = newReq.ImageName;
                    modal.Address = newReq.Address;
                    modal.IsActive = newReq.IsActive;

                    //var bookingDtls = _checkoutService.GetBookingByUserId(id ?? 0);
                    //if (bookingDtls != null)
                    //{
                        modal.TotalBookings =0;
                    //}

                    // modal.TotalBookings ??= bookingDtls.Count;
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView("_ViewFrontUser", modal);
        }
        #endregion [View Details]

        #region [ DELETE]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsDelete)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this User?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete User " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsDelete)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _frontUserService.GetFrontUserById(id);

                if (Details != null)
                {
                    Details.IsDeleted = true;
                    await _frontUserService.UpdateFrontUser(Details);
                    ShowSuccessMessage("Success!", "User deleted successfully.", false);
                    // return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "User deleted successfully.", Status = true, IsSuccess = true });
                }
                else
                {
                    ShowErrorMessage("Error!", "Error occurred, Please try again.", false);
                    //return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Some Error Occurred ", Status = false, IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index");

        }
        #endregion [ DELETE ]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsRead)]
        [HttpPost]
        //public IActionResult Export(MVE.DataTable.DataTables.DataTable request, bool? status, string? fStartDate, string? fEndDate, string txtSearchFilter)
        public IActionResult Export(MVE.DataTable.DataTables.DataTable requestE, bool? statusE, string? fStartDateE, string? fEndDateE, string txtSearchFilterE)
        {
            MVE.DataTable.DataTables.DataTable request = requestE;
            bool? status = statusE;
            string? fStartDate = fStartDateE;
            string? fEndDate = fEndDateE;
            string txtSearchFilter = txtSearchFilterE;

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("User");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "User ID";
                    worksheet.Cell(currentRow, 2).Value = "Full User Name";
                    worksheet.Cell(currentRow, 3).Value = "User Email Address";
                    worksheet.Cell(currentRow, 4).Value = "User Mobile Number";
                    worksheet.Cell(currentRow, 5).Value = "User Address";
                    // worksheet.Cell(currentRow, 4).Value = "Total User Bookings";
                    worksheet.Cell(currentRow, 6).Value = "User Status";
                    worksheet.Cell(currentRow, 7).Value = "User Date of Registration";


                    FrontUserViewModel vm = new FrontUserViewModel(request);
                    var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
                    var sortDirection = Request.Form["sSortDir_0"];

                    //Get filter query
                    var query = vm.FilterGrid(status, sortColumnIndex, sortDirection);


                    if (!string.IsNullOrEmpty(txtSearchFilter))
                    {
                        string sSearch = txtSearchFilter.ToLower();
                        var name_search = sSearch.Split(' ');
                        query.AddFilter(ad => (ad.FirstName.Trim() + " " + ad.LastName.Trim()).Contains(sSearch) ||
                 ad.Email.ToLower().Trim().Contains(sSearch.Trim()) ||
                 ad.Id.ToString().Contains(sSearch) ||
                 ad.MobilePhone.ToLower().Trim().Contains(sSearch) || ad.Address.ToLower().Trim().Contains(sSearch)
                 );
                    }
                    if (!string.IsNullOrEmpty(fStartDate) && !string.IsNullOrEmpty(fEndDate))
                    {
                        DateTime sDate = DateTime.ParseExact(fStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime eDate = DateTime.ParseExact(fEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        query.AddFilter(ad => ad.CreationOn >= sDate && ad.CreationOn <= eDate);
                        query.AddFilter(ad => ad.CreationOn >= sDate && ad.CreationOn <= eDate);
                    }

                    //get data
                    var frontUsers = _frontUserService.Search(query, out int totalCount).Entities.ToList();
                    vm.SetEntity(frontUsers);

                    vm.ComposeViewData();

                    if (frontUsers.ToList().Count <= 0)
                    {
                        //return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "No data found to export.", IsSuccess = false });
                        ShowErrorMessage("Error!", "No data found to export.", false);
                    }

                    for (int i = 0; i < frontUsers.ToList().Count; i++)
                    {
                        {
                            currentRow++;
                            worksheet.Cell(currentRow, 1).Value = frontUsers[i].Id;
                            worksheet.Cell(currentRow, 2).Value = frontUsers[i].FirstName + " " + frontUsers[i].LastName;
                            worksheet.Cell(currentRow, 3).Value = frontUsers[i].Email;
                            worksheet.Cell(currentRow, 4).Value = frontUsers[i].MobilePhone;
                            worksheet.Cell(currentRow, 5).Value = frontUsers[i].Address;
                            worksheet.Cell(currentRow, 6).Value = frontUsers[i].IsActive == true ? "Active" : "In-active";
                            worksheet.Cell(currentRow, 7).Value = frontUsers[i].CreationOn;

                        }
                    }

                    var builder = new StringBuilder();
                    List<string[]> data = new List<string[]>();
                    var rowh = new string[] { "Customer ID", "Customer Name", "Email Address", "Mobile Number", "Address", "Status", "Date of Registration" };
                    data.Add(rowh);
                    for (int i = 0; i < frontUsers.ToList().Count; i++)
                    {
                        //{
                        //    currentRow++;
                        //    worksheet.Cell(currentRow, 1).Value = frontUsers[i].Id;
                        //    worksheet.Cell(currentRow, 2).Value = frontUsers[i].FirstName + " " + frontUsers[i].LastName;
                        //    worksheet.Cell(currentRow, 3).Value = frontUsers[i].Email;
                        //    worksheet.Cell(currentRow, 4).Value = frontUsers[i].MobilePhone;
                        //    worksheet.Cell(currentRow, 5).Value = frontUsers[i].Address;
                        //    worksheet.Cell(currentRow, 6).Value = frontUsers[i].IsActive == true ? "Active" : "In-active";
                        //    worksheet.Cell(currentRow, 7).Value = frontUsers[i].CreationOn;

                        //}

                        var row = new string[] { frontUsers[i].Id.ToString(), frontUsers[i].FirstName + " " + frontUsers[i].LastName, frontUsers[i].Email, frontUsers[i].MobilePhone, frontUsers[i].Address, frontUsers[i].IsActive == true ? "Active" : "In-active", frontUsers[i].CreationOn.ToString() };
                        data.Add(row);
                    }

                    foreach (var row in data)
                    {
                        List<string> formattedRow = new List<string>();
                        for (int i = 0; i < row.Length; i++)
                        {
                            if (row[i] != null)
                            {
                                if (row[i].Contains(",") || row[i].Contains("\""))
                                {
                                    // If the field contains a comma or double quotes, enclose it within double quotes
                                    formattedRow.Add($"\"{row[i].Replace("\"", "\"\"")}\"");
                                }
                                else
                                {
                                    formattedRow.Add(row[i]);
                                }
                            }
                            else
                            {
                                formattedRow.Add("");
                            }
                        }

                        //builder.AppendLine(string.Join(",", row));
                        builder.AppendLine(string.Join(",", formattedRow));
                    }

                    byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());

                    // Return CSV file as a downloadable file
                    ShowSuccessMessage("Success!", "Data exported successfully.", false);

                    return File(bytes, "text/csv", "UserDetails.csv");


                    //using var stream = new MemoryStream();
                    //workbook.SaveAs(stream);
                    //var content = stream.ToArray();
                    //Response.Clear();
                    //Response.Headers.Add("content-disposition", "attachment;filename=UserDetails.csv");
                    //Response.ContentType = "application/csv";
                    //Response.Body.WriteAsync(content);
                    //Response.Body.Flush();
                    //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserDetails.csv");
                }
            }
            catch (Exception ex)
            {
                string exm = ex.Message;
                return File("", "text/csv", "UserDetails.csv");

            }
        }

        #region [ ADD / EDIT User]

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsCreate)]

        [HttpGet]
        public IActionResult AddEditUser(int? id)
        {
            FrontUserViewModel model = new FrontUserViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _frontUserService.GetFrontUserById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.FirstName = accObj.FirstName;
                    model.LastName = accObj.LastName;
                    model.DateOfBirth = accObj.DateOfBirth;
                    model.Address = accObj.Address;
                    model.MobilePhone = accObj.MobilePhone;
                    model.Email = accObj.Email;
                    model.IsActive = accObj.IsActive;
                    model.NewPassword = "Dots@123";
                    model.ConfirmPassword = "Dots@123";
                }
            }
            else
            {
                model.IsPassUpdate = true;
            }
            return PartialView("_AddEditFrontUser", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditUser(FrontUserViewModel model)
        {
            var msg = "";
            try
            {
                User user = null; bool isUpdate = false;
                var subject = "New User";
                if (model.Id > 0)
                {
                    subject = "User Password Change";
                    user = _frontUserService.GetFrontUserById(model.Id);
                    if (user != null)
                    {
                        isUpdate = true;
                        user.ModifiedOn = DateTime.UtcNow;
                        user.ModifiedBy = CurrentUser.Id;
                    }
                    msg = "";
                    msg = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/dark-logo.png\" style=\"height:50px\" /></p>    <p>Hello " + model.FirstName + ",</p>   " +
                   " <p>Your password has been changed for " + model.Email + " .<br> Here is your password. </p><br> New Password : "
                   + model.NewPassword + " <br> Here&#39;s your&nbsp;link to login: @@RESETLINK@@ <br>  <p>Kind regards,</p>   " +
                   " <p><strong>Europe Trip</strong></p>";
                }
                else
                {
                    user = new User();
                    user.CreationOn = DateTime.UtcNow;
                    user.CreatedBy = CurrentUser.Id;
                    var user1 = _frontUserService.GetFrontUserByEmail(model.Email);

                    if (user1 != null)
                    {
                        var displayMsg1 = "User is already registered";
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg1, Status = false, IsSuccess = false });
                    }
                    msg = "";
                    msg = "<p><img alt=\"Europe Trip\" src=\"@@SITEURL@@/images/dark-logo.png\" style=\"height:50px\" /></p>    <p>Hello " + model.FirstName + ",</p>   " +
                  " <p> Here is your user Id and password. </p><br>  User Id : " + model.Email.Trim() + " <br>  Password : " + model.NewPassword + " <br>  Here&#39;s your&nbsp;link to login: @@RESETLINK@@ <br>  <p>Kind regards,</p>   " +
                  " <p><strong>Europe Trip</strong></p>";
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.MobilePhone = model.MobilePhone;
                user.Address = model.Address;
                user.IsActive = model.IsActive;
                user.Email = model.Email;
                if (model.NewPassword != null && model.IsPassUpdate == true)
                {
                    user.SaltKey = PasswordEncryption.CreateSaltKey();
                    user.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.NewPassword, user.SaltKey);
                }

                user.DateOfBirth = model.DateOfBirth;
                user.City = model.City?.Trim();
                user.CountryId = model.CountryId;
                user.Gender = model.Gender;
                user.ZipCode = model.ZipCode?.Trim();
                user.StateOrCounty = model.StateOrCounty;
                user.IsEmailVerified = false;
                user.IsDeleted = false;
                user.EmailVerificationOtpExpired = DateTime.Now.AddDays(-24);

                user.ImageName = "";

                user = isUpdate ? await _frontUserService.UpdateFrontUser(user) : await _frontUserService.SaveFrontUser(user);

                if(!isUpdate)
                {
                    //SaveUserNotification
                    CreateNotification createNotification = new CreateNotification();
                    UserNotification userNotification = createNotification.CreateUserNotification(1, 1, 1, user.Id, 1, "User Registration", "Congratulations, Your registration on Europe Trip successfull.");
                    await _notificationService.SaveUserNotification(userNotification);
                    //EndSaveUserNotification

                    //SaveUserBilling
                    Billing billing = _userService.GetBillingByUserId(user.Id) ?? new Billing();
                    if (billing?.UserId <= 0)
                    {
                        billing.UserId = user.Id;
                        billing.CreatedBy = user.Id;
                        billing.CreationOn = DateTime.UtcNow;
                        await _userService.SaveBilling(billing);
                    }
                    //End SaveUserBilling
                }


                if (model.NewPassword != null && model.IsPassUpdate == true)
                {
                    await Task.Run(() => { _emailFactoryService.SendFrontUserEmail(msg, model.Email, subject); });
                }
                string displayMsg = $"User has been  {(isUpdate ? "updated" : "created")} successfully.";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, Status = true, IsSuccess = true });
                //ShowSuccessMessage("Success", displayMsg, false);
            }
            catch (Exception e)
            {
                // ShowErrorMessage("Error", "Some Error Occurred ", false);
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = e.Message, Message = e.Message, Status = false });
            }
            //return RedirectToAction("Index");
        }

        #endregion [ ADD / EDIT ]
        #region [Update Status Active/In-Active]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageUser, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> UserActiveInactive(long id, bool data, string coltype)
        {
            try
            {
                User entity = _frontUserService.GetFrontUserById(id);
                entity.IsActive = data;
                entity.ModifiedOn = DateTime.UtcNow;
                entity.ModifiedBy = CurrentUser.Id;
                await _frontUserService.UpdateFrontUser(entity);
                //SaveUserNotification
                CreateNotification createNotification = new CreateNotification();
                UserNotification userNotification = createNotification.CreateUserNotification(id, 1, 2, CurrentUser.Id, 1, "active/in-active user status", "Your account has been "+ (entity.IsActive==true? "activated" : "deactivated") + " by admin");
                await _notificationService.SaveUserNotification(userNotification);
                //EndSaveUserNotification

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"User  {(entity.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = message, IsSuccess = false });
            }
        }
        #endregion [Update Status Active/In-Active]
    }
}
