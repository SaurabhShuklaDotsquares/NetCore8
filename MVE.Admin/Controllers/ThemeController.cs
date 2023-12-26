using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.LIBS;
using MVE.Core.Models.Others;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVE.Core.Code.Attributes;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class ThemeController : BaseController
    {
        private readonly IThemeService _themeService;
        public ThemeController(IThemeService themeService)
        {
            _themeService = themeService;
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {
            ThemeViewModel vm = new ThemeViewModel();

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable, bool? status)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Theme>();
            if (status != null)
                query.AddFilter(b => b.IsActive == status);
            query.AddFilter(q => q.IsDeleted == false);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => q.Name.Contains(sSearch) || (q.Description ?? string.Empty).Contains(sSearch));
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Theme, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Theme, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Theme, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Theme, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
            IEnumerable<Theme> themeobj = _themeService.Get(query, out total).Entities;

            foreach (Theme r in themeobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                    count.ToString(),
                    r.ImageName??string.Empty,
                    r.Name,
                    r.Description??string.Empty,
                    r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        #region [ ADD / EDIT Theme]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public IActionResult AddEditTheme(int? id)
        {
            ThemeViewModel model = new ThemeViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _themeService.GetThemeById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.IsActive = accObj.IsActive;
                    model.ImageName = (string.IsNullOrEmpty(accObj.ImageName) ? "" : SiteKeys.UploadFilesTheme + accObj.ImageName);
                    model.File = new FormFile(null, 0, 0, SiteKeys.UploadFilesTheme + accObj.ImageName, accObj.ImageName);
                }
            }
            else
            {
                model.IsActive = true;

            }
            return PartialView("_AddEditTheme", model);
        }


        [HttpPost]
        public async Task<IActionResult> AddEditTheme(ThemeViewModel model)
        {
            try
            {
                Theme theme = null; bool isUpdate = false;
                if (model.Name != null && model.Id == 0)
                {
                    var isNameExist = IsNameExist(model);
                    if (isNameExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Theme '" + model.Name + "' is already exists." });
                    }
                }
                if (model.Id > 0)
                {
                    theme = _themeService.GetThemeById(model.Id);
                    if (theme != null)
                    {
                        isUpdate = true;
                        theme.ModifiedOn = DateTime.UtcNow;
                        theme.ModifiedBy = CurrentUser.Id;
                    }
                }
                else
                {
                    theme = new Theme();
                    theme.CreatedOn = DateTime.UtcNow;
                    theme.CreatedBy = CurrentUser.Id;
                }
                if (model.File?.FileName != null)
                {
                    CommonFileViewModel.FileUpload(model.File, SiteKeys.UploadFilesTheme);
                    theme.ImageName = model.File.FileName;

                }
                theme.Name = model.Name;
                theme.Description = model.Description;
                theme.IsActive = model.IsActive;

                string input = model.Name;
                theme.ShortName = input?.Replace(" ", "_").ToLower();


                var isshortNameExist = IsShortNameExist(theme.ShortName, model.Id);
                if (isshortNameExist)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Theme short name is already exists." });
                }


                theme = isUpdate ? await _themeService.UpdateTheme(theme) : await _themeService.SaveTheme(theme);
                string displayMsg = $"Theme has been  {(isUpdate ? "updated" : "created")} successfully";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, Status = true, IsSuccess = true });

            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = e.Message, Message = e.Message, Status = false });

            }
        }


        #endregion [ ADD / EDIT ]
        #region [ EXISTS ]

        [HttpPost]
        public JsonResult IsThemeExists(string Name, string PreInitialName)
        {
            bool isExist = false;
            if (Name != PreInitialName)
            {
                isExist = _themeService.IsThemeExists(Name);
            }
            return Json(!isExist);
        }
        public bool IsNameExist(ThemeViewModel model)
        {
            bool isExist = false;

            isExist = _themeService.IsThemeExists(model.Name);

            return isExist;
        }
        public bool IsShortNameExist(string _ShortName, int ids)
        {
            bool isExist = false;

            isExist = _themeService.IsThemeShortNameExists(_ShortName, ids);

            return isExist;
        }
        #endregion [ EXISTS ]

        #region [ UPDATE STATUS ]      
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            try
            {
                var themeObj = _themeService.GetThemeById(Convert.ToInt32(id));
                if (themeObj == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                themeObj.IsActive = !themeObj.IsActive;
                themeObj.ModifiedOn = DateTime.UtcNow;
                themeObj.ModifiedBy = CurrentUser.Id;
                await _themeService.UpdateTheme(themeObj);

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $" Theme Status  {(themeObj.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }


        }
        #endregion [ UPDATE STATUS ]

        #region [ DELETE ]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsDelete)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Theme " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _themeService.GetThemeById(id);

                if (Details != null)
                {
                    Details.IsDeleted = true;
                    await _themeService.UpdateTheme(Details);
                    ShowSuccessMessage("Success!", "Theme deleted successfully.", false);
                }
                else
                {
                    ShowErrorMessage("Error!", "Some Error Occurred.", false);
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
        public ActionResult Thumbnail(int width, int height, string imageFile)
        {
            try
            {
                imageFile = $"{Directory.GetCurrentDirectory()}\\wwwroot{SiteKeys.UploadFilesTheme}{imageFile}";
                // TODO: the filename could be passed as argument of course
                using (var srcImage = Image.FromFile(imageFile))
                using (var newImage = new Bitmap(width, height))
                using (var graphics = Graphics.FromImage(newImage))
                using (var stream = new MemoryStream())
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new System.Drawing.Rectangle(0, 0, width, height));
                    newImage.Save(stream, ImageFormat.Png);
                    return File(stream.ToArray(), "image/png");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
