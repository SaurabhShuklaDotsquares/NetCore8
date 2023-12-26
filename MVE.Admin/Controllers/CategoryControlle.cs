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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;
using MVE.Core.Code.Attributes;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MVE.Admin.Models;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsRead)]
        public IActionResult Index(int? id)
        {
            CategoryViewModel vm = new CategoryViewModel();
            vm.ParentId = Convert.ToInt32(id);
            ViewBag.delete = "Country deleted successfully.";
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable, bool? status, string? fStartDate, string? fEndDate,int? ParentId)
        {
            ParentId = Convert.ToInt32(ParentId);

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Categories>();
            if (status != null)
                query.AddFilter(b => b.IsActive == status);
            query.AddFilter(q => q.IsDeleted == false);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => q.Name.Contains(sSearch) || (q.Description ?? string.Empty).Contains(sSearch));
            }
            query.AddFilter(q => q.ParentId== ParentId);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;

            IEnumerable<Categories> Categorysobj = _categoryService.Get(query, out total).Entities;
        
            foreach (Categories r in Categorysobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                     count.ToString(),
                     r.Image??"", // string.IsNullOrEmpty(r.Image)?"No Image":r.Image,
                      r.Name,
                    r.Description??string.Empty,
                     r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    _categoryService.CountChildCategory(r.Id).ToString(),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }


        public IActionResult SubCategory()
        {
            CategoryViewModel vm = new CategoryViewModel();
            return View("SubCategory");
        }
        [HttpPost]
        public ActionResult SubCategory(MVE.DataTable.DataTables.DataTable dataTable, bool? status, string? fStartDate, string? fEndDate,int? id)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<Categories>();
            if (status != null)
                query.AddFilter(b => b.IsActive == status);
            query.AddFilter(q => q.IsDeleted == false);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                query.AddFilter(q => q.Name.Contains(sSearch) || (q.Description ?? string.Empty).Contains(sSearch));
            }
            query.AddFilter(q => q.ParentId == id);
            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Categories, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;

            IEnumerable<Categories> Categorysobj = _categoryService.Get(query, out total).Entities;

            foreach (Categories r in Categorysobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                     count.ToString(),
                     r.Image??"", // string.IsNullOrEmpty(r.Image)?"No Image":r.Image,
                      r.Name,
                    r.Description??string.Empty,
                     r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
                    _categoryService.CountChildCategory(r.Id).ToString(),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
        public static DateTime ParsestringDateintoDatetime(string date)
        {
            DateTime dt = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            return dt;
        }

        #region [ ADD / EDIT Category]

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsCreate)]

        [HttpGet]
        public IActionResult AddEditCategory(int? id)
        {
            CategoryViewModel model = new CategoryViewModel();
            var catLst = _categoryService.GetParentCategoryForDropDown();
            //ViewBag.RolesListForDropDown = roles;
            model.ParentCategoryList = catLst;
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _categoryService.GetCategoryById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.IsActive = accObj.IsActive ? accObj.IsActive : false;
                    model.Image = (string.IsNullOrEmpty(accObj.Image) ? "" : SiteKeys.UploadFilesCategory + accObj.Image);
                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_AddEditCategory", model);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult ViewCategory(int? id)
        {
            CategoryViewModel model = new CategoryViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _categoryService.GetCategoryById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.Image = (string.IsNullOrEmpty(accObj.Image) ? "" : SiteKeys.UploadFilesCategory + accObj.Image);
                    //model.Image = new FormFile(null, 0, 0, SiteKeys.UploadFilesCategory + accObj.Image, accObj.Image);
                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_ViewCategory", model);
        }


        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> AddEditCategory(CategoryViewModel model)
        {
            try
            {
                Categories Category = null; bool isUpdate = false;
                if (model.Name != null && model.Id == 0)
                {
                    var isNameExist = IsNameExist(model);
                    if (isNameExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Category '" + model.Name + "' is already exists.", IsSuccess = false });
                    }
                }
                if (model.Id > 0)
                {
                    Category = _categoryService.GetCategoryById(model.Id);
                    if (Category != null)
                    {
                        isUpdate = true;
                        Category.ModifiedOn = DateTime.UtcNow;
                        Category.ModifiedBy = CurrentUser.Id;
                    }
                }
                else
                {
                    Category = new Categories();
                    Category.CreatedOn = DateTime.UtcNow;
                    Category.CreatedBy = CurrentUser.Id;
                }
                if (model.ImageFile?.FileName != null)
                {
                    CommonFileViewModel.FileUpload(model.ImageFile, SiteKeys.UploadFilesCategory);
                    Category.Image = model.ImageFile.FileName;

                }
                Category.Name = model.Name;
                Category.ParentId = model.ParentId;
                Category.Description = model.Description;
               
                Category.IsActive = model.IsActive;


                Category = isUpdate ? await _categoryService.UpdateCategory(Category) : await _categoryService.SaveCategory(Category);


                string displayMsg = $"Category has been  {(isUpdate ? "updated" : "created")} successfully";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = e.Message, IsSuccess = false });

            }
        }

        #endregion [ ADD / EDIT ]
        #region [ EXISTS ]

       
        public bool IsNameExist(CategoryViewModel model)
        {
            bool isExist = false;

            isExist = _categoryService.IsCategoryExists(model.Name);

            return isExist;
        }

        #endregion [ EXISTS ]

        #region [ UPDATE STATUS - Category]   
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var CategoryObj = _categoryService.GetCategoryById(Convert.ToInt32(id));
                if (CategoryObj == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                CategoryObj.IsActive = !CategoryObj.IsActive;
                CategoryObj.ModifiedOn = DateTime.UtcNow;
                CategoryObj.ModifiedBy = CurrentUser.Id;
                await _categoryService.UpdateCategory(CategoryObj);

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"Category  {(CategoryObj.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }
        }
        #endregion [ UPDATE STATUS ]

        #region [ DELETE Category]

        [HttpGet]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageCategory, AppPermissions.Action_IsDelete)]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Category?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Category " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _categoryService.GetCategoryById(id);

                if (Details != null)
                {
                    Details.IsDeleted = true;
                   // await _categoryService.UpdateCategory(Details);
                    ViewBag.delete = "Country deleted successfully.";
                    ShowSuccessMessage("Success!", "Country deleted successfully.", false);
                }
                else
                {
                    ViewBag.delete = "Some Error Occurred.";
                    ShowErrorMessage("Error!", "Some Error Occurred ", false);
                }
            }
            catch (Exception ex)
            {
                
                string message = ex.GetBaseException().Message;
                ViewBag.delete = message;
                ShowErrorMessage("Error!", message, false);
            }
            return View("Index");

        }
        #endregion [ DELETE ]
        public ActionResult Thumbnail(int width, int height, string imageFile)
        {
            try
            {
                imageFile = Directory.GetCurrentDirectory() + "/wwwroot/" + SiteKeys.UploadFilesCategory + imageFile;
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
