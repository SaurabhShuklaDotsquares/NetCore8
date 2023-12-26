using Microsoft.AspNetCore.Mvc;
using System.Data;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.Attributes;
using MVE.Core.Code.LIBS;
using MVE.Core.Models;
using MVE.Core.Models.Others;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace MVE.Admin.Controllers
{
    public class UserRoleController : BaseController
    {
        #region [ SERVICE INJECTION ]
        private readonly IUserRoleService _userRoleService;
        private readonly IAdminUserService _adminUserService;
        private readonly IPermissionService _permissionService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UserRoleController(IUserRoleService userRoleService, IAdminUserService adminUserService, IPermissionService permissionService, IHostingEnvironment hostingEnvironment)
        {
            _userRoleService = userRoleService;
            _adminUserService = adminUserService;
            _permissionService = permissionService;
            _hostingEnvironment = hostingEnvironment;
        }

        #endregion [ SERVICE INJECTION ]

        #region [ INDEX ]
        /// <summary>
        /// Navigate & Start From This Index View
        /// </summary>
        /// <returns>return to Index View</returns>
        /// 
        [CustomAuthorization(AppPermissions.Pages_Administration_Roles, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult Index()
        {
            UserRoleViewModel model = new UserRoleViewModel();
            model.RoleType = CurrentUser.Role;
            return View(model);
        }

        /// <summary>
        /// Get & Set role Table record into DataTable With Pagination
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns>return role Details DataTable With Pagination</returns>
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable)
        {

            List<DataTableRow> table = new List<DataTableRow>();    

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<UserRole>();
            //query.AddFilter(q => q.Name != "Administrator");
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => q.RoleName.Contains(sSearch));
            }

            query.AddFilter(q => q.IsDeleted == false);

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserRole, string>(q => q.RoleName, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserRole, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<UserRole, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<UserRole> roles = _userRoleService.Get(query, out total).Entities;

            foreach (UserRole r in roles)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {                  
                    r.Id.ToString(),
                    count.ToString(),
                    r.RoleName,
                    r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        #endregion [ INDEX ]

        #region [ ADD / EDIT ]
        /// <summary>
        /// Get & Set Value into UserRoleModel With AddEdit Partial View
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return AddEdit Partial View</returns>
        [CustomAuthorization(AppPermissions.Pages_Administration_Roles, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public IActionResult AddEdit(long? id)
        {
            UserRoleViewModel model = new UserRoleViewModel();
            if (id.HasValue)
            {
                var role = _userRoleService.GetUserRoleById(id.Value);
                if (role != null)
                {
                    model.Id = Convert.ToString(role.Id);
                    model.Name = role.RoleName;
                    model.PreInitialRoleName = role.RoleName;
                    model.IsActive = role.IsActive ? role.IsActive : false;
                    model.CreatedOn = role.CreatedOn;

                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_AddEdit", model);
        }

        /// <summary>
        /// Insert or Update UserRoleModel View Model Record into DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns>return Json With Message</returns>
        /// 
        [CustomAuthorization(AppPermissions.Pages_Administration_Roles, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> AddEdit(UserRoleViewModel model)
        {
            UserRole role = null; bool isUpdate = false;
            if (model.Name != null && Convert.ToInt32(model.Id) == 0)
            {
                var isNameExist = IsNameExist(model);
                if (isNameExist)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Role '" + model.Name + "' is already exists." });
                }
            }
            if (Convert.ToInt32(model.Id) > 0)
            {
                role = _userRoleService.GetUserRoleById(Convert.ToInt64(model.Id));
                if (role != null)
                {
                    isUpdate = true;
                }
            }
            else
            {
                role = new UserRole();
                role.CreatedOn = DateTime.UtcNow;
            }
            role.RoleName = model.Name;
            role.IsDeleted = false;
            role.IsActive = Convert.ToBoolean(model.IsActive);
            role.ModifiedOn = DateTime.UtcNow;
            role = isUpdate ? await _userRoleService.UpdateUserRole(role) : await _userRoleService.SaveUserRole(role);


            string displayMsg = $"User Role has been  {(isUpdate ? "updated" : "created")} successfully";
          
            ShowSuccessMessage("Success!", "Added successfully.", false);
            //return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, Status = true });
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Role Added successfully.", IsSuccess = true, RedirectUrl = "/UserRole/Index" });


        }

        #endregion [ ADD / EDIT ]

        #region [ EXISTS ]
        /// <summary>
        /// Compare Previous Role Name With Enter Name Exists into DB or Not
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="PreInitialEmail"></param>
        /// <returns>return Boolean</returns>
        [HttpPost]
        public JsonResult IsRoleNameExist(string Name, string PreInitialRoleName)
        {
            bool isExist = false;
            if (Name != PreInitialRoleName)
            {
                isExist = _userRoleService.IsRoleExists(Name);
            }
            return Json(!isExist);
        }
        public bool IsNameExist(UserRoleViewModel model)
        {
            bool isExist = false;

            isExist = _userRoleService.IsRoleExists(model.Name);

            return isExist;
        }

        #endregion [ EXISTS ]

        #region [ UPDATE STATUS ]
        /// <summary>
        /// Active / InActive Status Of Role
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Json With Message</returns>
        /// 
        [CustomAuthorization(AppPermissions.Pages_Administration_Roles, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var role = _userRoleService.GetUserRoleById(Convert.ToInt64(id));
                if (role == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                role.ModifiedOn = DateTime.UtcNow;
                role.IsActive = !role.IsActive;
                await _userRoleService.UpdateUserRole(role);
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"Role {(role.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }
        }
        #endregion [ UPDATE STATUS ]

        #region [ DELETE ]
        /// <summary>
        /// Show Confirmation Box For Delete Record
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return Delete Confirmation Box </returns>

        [CustomAuthorization(AppPermissions.Pages_Administration_Roles, AppPermissions.Action_IsDelete)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Role?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Role" },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }
        /// <summary>
        /// Delete Record From DB
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FC"></param>
        /// <returns>return Json With Message</returns>

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var roleDetails = _userRoleService.GetUserRoleById(Convert.ToInt64(id));
                bool isRoleInUse = _adminUserService.IsRoleInUse(Convert.ToInt64(id));
                if (!isRoleInUse)
                {
                    if (roleDetails != null)
                    {
                        roleDetails.IsDeleted = true;
                        await _userRoleService.UpdateUserRole(roleDetails);
                        ShowSuccessMessage("Success!", "Role deleted successfully.", false);
                    }
                    else
                    {
                        ShowErrorMessage("Error!", "Error Occurred Please try sometime or contact to administrator. ", false);
                    }
                }
                else
                {
                    ShowErrorMessage("Error!", roleDetails.RoleName + " role is already in use, so can't delete", false);
                }
            }
            catch (Exception ex)
            {
                string message = ex.GetBaseException().Message;
                ShowErrorMessage("Error!", message, false);
            }
            return RedirectToAction("Index");


            //string message = string.Empty;
            //message = _roleService.DeleteRole(id);
            //if (string.IsNullOrEmpty(message))
            //{
            //    ShowSuccessMessage("Success!", "Role deleted successfully.", false);
            //}
            //else
            //{
            //    ShowErrorMessage("Error!", message, false);
            //}
            //return RedirectToAction("Index");
        }
        #endregion [ DELETE ]

        #region [ PERMISSIONS  OLD]

        //[CustomAuthorization(AppPermissions.Pages_Administration_Roles_Edit)]
        //public IActionResult Permission(long? id)
        //{
        //    UserRoleViewModel model = new UserRoleViewModel();
        //    if (id.HasValue)
        //    {
        //        var roleEntity = _userRoleService.GetUserRoleById(id.Value);
        //        if (roleEntity != null)
        //        {
        //            model.Id = Convert.ToString(roleEntity.Id);
        //            model.Name = roleEntity.RoleName;
        //            model.PreInitialRoleName = roleEntity.RoleName;
        //            model.IsActive = roleEntity.IsActive ? roleEntity.IsActive : false;
        //            model.CreatedOn = roleEntity.CreatedOn;
        //            if (roleEntity.ModifiedOn == null)
        //            {
        //                model.ModifiedOn = roleEntity.CreatedOn;
        //            }
        //            else
        //            {
        //                model.ModifiedOn = roleEntity.ModifiedOn;
        //            }
        //            model.RolePermissionList = roleEntity.UserPermissions?.Select(x => new PermissionViewModel
        //            {
        //                Id = x.Id,
        //                RoleId = x.RoleId,
        //                PermissionId = x.UserPermissionId,
        //            }).ToList() ?? new List<PermissionViewModel>();

        //            model.PermissionSelectedIds = string.Join(",", model.RolePermissionList.Select(x => x.PermissionId.ToString()).ToArray());
        //        }
        //    }

        //    return View(model);
        //}

        //public ActionResult GetPermissions(int? id)
        //{
        //    if (id > 0)
        //    {
        //        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(AppPermissionTree.Permissions));
        //    }
        //    else
        //        return Content(Newtonsoft.Json.JsonConvert.SerializeObject(AppPermissionTree.Permissions));

        //}

        //[CustomAuthorization(AppPermissions.Pages_Administration_Roles_Edit)]
        //[HttpPost]
        //public IActionResult Permission(UserRoleViewModel model)
        //{
        //    bool isUpdate = false;
        //    try
        //    {
        //        ModelState.Remove("RoleType");
        //        ModelState.Remove("PreInitialRoleName");
        //        if (ModelState.IsValid)
        //        {
        //            if (Convert.ToInt32(model.Id) > 0)
        //            {
        //                var role = _userRoleService.GetUserRoleById(Convert.ToInt64(model.Id));
        //                if (role != null)
        //                {
        //                    isUpdate = _permissionService.DeletePermission(role.UserPermissions.ToList());
        //                    role.UserPermissions.ToList().Clear();
        //                    if (!string.IsNullOrEmpty(model.PermissionSelectedIds))
        //                    {
        //                        var Permissions = model.PermissionSelectedIds?.Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).Select(x =>
        //                                           new UserPermission
        //                                           {
        //                                               RoleId = role.Id,
        //                                               UserPermissionId = Convert.ToInt32(x),
        //                                               CreatedOn = DateTime.Now,
        //                                               CreatedBy = CurrentUser.Id
        //                                           }).ToList() ?? new List<UserPermission>();

        //                        _permissionService.InsertPermissionList(Permissions);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowErrorMessage("Error", ex.ToString(), false);
        //    }


        //    string displayMsg = $"Permission has been  {(isUpdate ? "updated" : "created")} successfully";
        //    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg });
        //}

        #endregion [ PERMISSIONS OLD ]

        #region [ PERMISSIONS ]

        public IActionResult Permission(long id)
        {
            RolePermissionViewModel vm = new RolePermissionViewModel();
            try
            {
                var roleEntity = _userRoleService.GetUserRoleById(id) ?? new UserRole();
                vm.SetEntityRole(roleEntity);

                var entity = _userRoleService.GetALLRolePages();
                vm.SetEntity(entity);

                vm.SetEntity(CurrentUser);

                vm.SetHostingEnvironment(_hostingEnvironment);

                vm.ComposeViewData();

                vm.RoleId = roleEntity.Id;
                vm.RoleName = roleEntity.RoleName;
            }
            catch (Exception ex)
            {

            }

            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Permission(RolePermissionViewModel model)
        {
            bool isUpdate = false;
            try
            {
                model.SetEntity(CurrentUser);
                model.SetHostingEnvironment(_hostingEnvironment);

                var rolePermissions = _userRoleService.GetRolePagePermissionsByRoleId(model.RoleId);

                if (rolePermissions != null)
                {
                    isUpdate = await _userRoleService.DeletePermission(rolePermissions.ToList());
                }

                if (model.RolePermissionList != null)
                {
                    List<RolePagePermission> rolePagePermissions = new List<RolePagePermission>();
                    foreach (var item in model.RolePermissionList)
                    {
                        RolePagePermission newPermisssion = new RolePagePermission();
                        newPermisssion.PageId = item.RolePageId;
                        newPermisssion.RoleId = item.RoleId;
                        newPermisssion.IsReadOnly = item.IsReadOnly;
                        newPermisssion.IsCreate = item.IsCreate;
                        newPermisssion.IsEdit = item.IsEdit;
                        newPermisssion.IsDelete = item.IsDelete;
                        rolePagePermissions.Add(newPermisssion);
                    }

                    await _userRoleService.InsertPermissionList(rolePagePermissions);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error", ex.ToString(), false);
            }

            ShowSuccessMessage("Success!", "Permission has been assigned successfully.", false);

            return RedirectToAction("index");
        }


        #endregion [ PERMISSIONS ]
    }
}
