using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVE.Core.Code.LIBS;
using MVE.Core;
using MVE.Admin.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using MVE.Service;
using MVE.DataTable.Extension;
using MVE.Core.Code.Attributes;
using MVE.Data.Models;
using System.Net;
using System.Reflection;
using MVE.Core.Models.Others;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class AdminUserController : BaseController
    {
        #region [ SERVICE INJECTION ]
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IAdminUserService _adminUserService;
        private readonly IUserRoleService _userRoleService;
        public AdminUserController(IConfiguration configuration, IHostingEnvironment hostingEnvironment, IAdminUserService adminUserService, IUserRoleService userRoleService)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _adminUserService = adminUserService;
            _userRoleService = userRoleService;
        }

        #endregion [ SERVICE INJECTION ]

        #region [ INDEX ]
        [CustomAuthorization(AppPermissions.Pages_Administration_AdminUsers, AppPermissions.Action_IsRead)]
        [HttpGet]
        [Route("[Controller]/[Action]")]
        public IActionResult Index()
        {
            AdminUserViewModel model = new AdminUserViewModel();
            var roles = _userRoleService.GetUserRolesForDropDown();
            //ViewBag.RolesListForDropDown = roles;
            model.roleDropDownList = roles;
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(MVE.DataTable.DataTables.DataTable request, bool? status, long? userRoleTypeId, string txtSearchFilter)
        {
            AdminUserViewModel vm = new AdminUserViewModel(request);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];

            //Get filter query
            var query = vm.FilterGrid(status, sortColumnIndex, sortDirection);

            if (userRoleTypeId > 0)
            {
                query.AddFilter(ad => ad.RoleId == userRoleTypeId);
            }
            if (!string.IsNullOrEmpty(txtSearchFilter))
            {
                string sSearch = txtSearchFilter.ToLower();
                var name_search = sSearch.Split(' ');
                query.AddFilter(ad => (ad.FirstName + " " + ad.LastName).Contains(sSearch) ||
                 ad.Email.ToLower().Contains(sSearch.Trim()) || ad.Role.RoleName.ToLower().Contains(sSearch.Trim()));
            }


            //get data
            var adminUsers = _adminUserService.Search(query, out int totalCount).Entities;

            vm.SetEntity(adminUsers);

            vm.ComposeViewData();

            return new DataTableResultExt(request, vm.GridViewData.Count(), totalCount, vm.GridViewData);
        }

        #endregion [ INDEX ]


        #region  [ CHANGE PASSWORD ]       
        [CustomAuthorization(AppPermissions.Pages_Administration_ChangePassword, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult ChangePassword(long id)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.UserId = Convert.ToString(CurrentUser.Id);
            return View(model);
        }
        [HttpGet]
        public IActionResult IsPasswordExist(string OldPassword, string UserId)
        {
            bool isPasswordExist = false;
            var user = _adminUserService.GetAdminUserById(Convert.ToInt64(UserId));
            if (user != null && (user.IsActive == true))
            {
                if (PasswordEncryption.IsPasswordMatch(user.EncryptedPassword, OldPassword, user.SaltKey))
                {
                    isPasswordExist = true;
                }
            }
            return Json(isPasswordExist);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _adminUserService.GetAdminUserById(Convert.ToInt64(model.UserId));
                    if (user != null)
                    {
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.NewPassword, user.SaltKey);
                        _adminUserService.UpdateAdminUser(user);
                        displayMsg = $"Password has been updated successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                displayMsg = ex.ToString();
            }
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg });
        }

        #endregion   CHANGE PASSWORD 

        #region [Manage]
        [HttpGet]
        public IActionResult Manage(long id, string type)
        {
            AdminUserManageViewModel model = new AdminUserManageViewModel();
            if (type == "edit")
            {
                model.Id = CurrentUser.Id;
            }
            else
            {
                model.Id = id;
            }
            model.TypeMode = type;
            if (model.Id > 0)
            {
                var user = _adminUserService.GetAdminUserById(model.Id);
                if (user != null)
                {
                    model.RoleId = user.RoleId;
                    model.FName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Email = user.Email;
                    model.MobilePhone = user.MobilePhone;
                    model.Address = user.Address;
                    model.Description = user.Description;
                }
            }
            var roles = _userRoleService.GetUserRolesForDropDownNotSuperAdmin();
            model.roleDropDownList = roles;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Manage(AdminUserManageViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                if (model.Email != null && Convert.ToInt32(model.Id) == 0)
                {
                    var isNameExist = IsNameExist(model);
                    if (isNameExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "User '" + model.Email + "' is already exists.", Status = false });
                    }
                }
                if (model.Id > 0)
                {
                    var user = _adminUserService.GetAdminUserById(model.Id);
                    if (user != null)
                    {
                        if (model.TypeMode != "edit")
                        {
                            user.RoleId = model.RoleId;
                            if (model.IsPassUpdate == true)
                            {
                                user.SaltKey = PasswordEncryption.CreateSaltKey();
                                user.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.Password, user.SaltKey);
                            }
                        }
                        user.FirstName = model.FName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;
                        user.MobilePhone = model.MobilePhone;
                        user.Address = model.Address;
                        user.Description = model.Description;
                        user.ModifiedBy = CurrentUser.Id;
                        user.ModifiedOn = DateTime.UtcNow;
                        await _adminUserService.UpdateAdminUserAsync(user);
                        displayMsg = $"User has been updated successfully.";
                    }
                }
                else
                {
                    AdminUser user = new AdminUser();
                    user.RoleId = model.RoleId;
                    user.FirstName = model.FName;
                    user.LastName = model.LastName;
                    user.Email = model.Email;
                    user.MobilePhone = model.MobilePhone;
                    user.Address = model.Address;
                    user.Description = model.Description;
                    user.CreatedBy = CurrentUser.Id;
                    user.SaltKey = PasswordEncryption.CreateSaltKey();
                    user.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.Password, user.SaltKey);
                    await _adminUserService.SaveAdminUserAsync(user);
                    displayMsg = $"User has been saved successfully.";
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.ToString(), IsSuccess = false });

            }
            if (model.TypeMode == "edit")
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, Status = true, IsSuccess = true });
            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
            }

        }
        public bool IsNameExist(AdminUserManageViewModel model)
        {
            bool isExist = false;

            isExist = _adminUserService.IsAdminUserExists(model.Email);

            return isExist;
        }
        #endregion [ Manage ]

        #region [ DELETE ]

        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete User " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var user = _adminUserService.GetAdminUserById(id);

                if (user != null)
                {
                    user.IsDeleted = true;
                    await _adminUserService.UpdateAdminUserAsync(user);
                    ShowSuccessMessage("Success!", "User deleted successfully.", false);
                }
                else
                {
                    ShowErrorMessage("Error!", "Some Error Occurred ", false);
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


        #region [ UPDATE STATUS ]      
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var user = _adminUserService.GetAdminUserById(id);
                if (user == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                user.IsActive = !user.IsActive;
                user.ModifiedOn = DateTime.UtcNow;
                user.ModifiedBy = CurrentUser.Id;
                await _adminUserService.UpdateAdminUserAsync(user);

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"User Status {(user.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }


        }
        #endregion [ UPDATE STATUS ]
        [HttpGet]
        public IActionResult ViewUserDetails(int? id)
        {
            AdminPersonalDetailsViewModel model = new AdminPersonalDetailsViewModel();
            if (id.HasValue)
            {
                var user = _adminUserService.GetAdminUserById(id.Value);
                if (user != null)
                {
                    model.RoleName = user.Role.RoleName;
                    model.FullName = CommonFileViewModel.GetFullName(user.FirstName, user.LastName);
                    model.Email = user.Email;
                    model.MobileNo = user.MobilePhone;
                    model.Address = user.Address;
                }
            }
            return PartialView("_ViewUser", model);
        }
    }
}
