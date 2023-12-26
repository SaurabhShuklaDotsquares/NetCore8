using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;
using MVE.Service.Banner;
using MVE.Core;
using MVE.Admin.ViewModels;
using MVE.Core.Code.Attributes;
using MVE.Admin.ViewModels.StaticContentBanner;
using DocumentFormat.OpenXml.Office2010.Excel;
using MVE.Core.Models.Others;
using MVE.Core.Code;
using System.ComponentModel;
using System.Reflection;
//using System.Reflection;
//using System.ComponentModel;

namespace MVE.Admin.Controllers
{
    public class BannerController : BaseController
    {
        private readonly IStaticContentBannerService _staticContentBannerService;
        public BannerController(IStaticContentBannerService staticContentBannerService)
        {
            _staticContentBannerService = staticContentBannerService;
        }
        #region [Index]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {

            return View();
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
            var query = new SearchQuery<StaticContentBanner>();
            if (status != null)
                query.AddFilter(b => b.IsActive == status);
            query.AddFilter(q => q.IsDeleted == false);
            if (!string.IsNullOrEmpty(dataTable.sSearch))
            {
                string sSearch = dataTable.sSearch.Trim().ToLower();
                var imgforid = (int)Enum.GetValues(typeof(ImageFor)).Cast<ImageFor>().FirstOrDefault(e => e.GetDescription().ToLower().Contains(sSearch));
                    query.AddFilter(q => q.ImageFor == imgforid);
            }

            var sortColumnIndex = Convert.ToInt32(Request.Form["iSortCol_0"]);
            var sortDirection = Request.Form["sSortDir_0"];
            switch (sortColumnIndex)
            {
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticContentBanner, string>(q => q.ImageName, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticContentBanner, DateTime>(q => q.CreatedDate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticContentBanner, bool>(q => q.IsActive ?? false, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<StaticContentBanner, DateTime>(q => q.CreatedDate, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;

            var countrysobj = _staticContentBannerService.Get(query, out total).Entities;

            foreach (StaticContentBanner r in countrysobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                     count.ToString(),
                    ((ImageFor)r.ImageFor).GetDescription(),
                     r.ImageName?? "", // string.IsNullOrEmpty(r.Image)?"No Image":r.Image,
                    r.CreatedDate.ToString(SiteKeys.DateFormatWithoutTime),
                    r.IsActive.ToString(),
                });
                count++;
            }
            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsRead)]
        public static string GetDescription(ImageFor value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute != null ? attribute.Description : value.ToString();
        }
        #endregion

        #region [ ADD / EDIT Banner]

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult AddEditBanner(int? id)
        {
            StaticContentBannerViewModels model = new StaticContentBannerViewModels();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _staticContentBannerService.GetStaticContentBannerById(Convert.ToInt32(id));
                if (accObj != null)
                {
                    //var data= Enum.GetNames(typeof(ImageFor));
                    model.Id = accObj.Id;
                    model.ImageFor = accObj.ImageFor;
                    model.IsActive = accObj.IsActive ??  false;
                    model.FileName = (string.IsNullOrEmpty(accObj.ImageName) ? "" : SiteKeys.UploadFilesBanner + accObj.ImageName);
                    model.FlagImage = new FormFile(null, 0, 0, SiteKeys.UploadFilesBanner + accObj.ImageName, accObj.ImageName);
                }
            }
            else
            {
                model.IsActive
                    = false;
            }
            return PartialView("_AddEditBanner", model);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult Viewbanner(int? id)
        {
            StaticContentBannerViewModels model = new StaticContentBannerViewModels();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _staticContentBannerService.GetStaticContentBannerById(Convert.ToInt32(id));
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.ImageName = ((ImageFor)accObj.ImageFor).GetDescription();
                    model.FileName = (string.IsNullOrEmpty(accObj.ImageName) ? "" : SiteKeys.UploadFilesBanner + accObj.ImageName);
                    model.FlagImage = new FormFile(null, 0, 0, SiteKeys.UploadFilesBanner + accObj.ImageName, accObj.ImageName);
                }
            }
            return PartialView("_Viewbanner", model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEditBanner(StaticContentBannerViewModels model)
        {
            try
            {
                StaticContentBanner banner = null; bool isUpdate = false;
                if (model.Id > 0)
                {
                    banner = _staticContentBannerService.GetStaticContentBannerById(Convert.ToInt32(model.Id));
                    if (banner != null)
                    {
                        isUpdate = true;
                        banner.ModifiedDate = DateTime.UtcNow;
                        banner.ModifiedBy = CurrentUser.Id;
                        banner.IsDeleted = banner.IsDeleted;
                    }
                    else
                    {
                        banner.IsDeleted = false;
                    }
                }
                else
                {
                    banner = new StaticContentBanner();
                    banner.CreatedDate = DateTime.UtcNow;
                    banner.CreatedBy = CurrentUser.Id;
                }
                if (model.FlagImage?.FileName != null)
                {
                    
                    CommonFileViewModel.FileUpload(model.FlagImage, SiteKeys.UploadFilesBanner);
                    banner.ImageName = model.FlagImage.FileName;

                }
                else if (model.ImageFor>0)
                {
                    banner.ImageFor= banner.ImageFor;
                }
                banner.IsActive = model.IsActive;
                banner.ImageFor = model.ImageFor;
                banner.ImageExtension = model.FlagImage?.FileName!=null?Path.GetExtension(model.FlagImage?.FileName):"";
                banner.ImageType = model.ImageType;

                banner = isUpdate ? await _staticContentBannerService.UpdateStaticContentBanner(banner) : await _staticContentBannerService.SaveStaticContentBanner(banner);


                string displayMsg = $"Banner has been  {(isUpdate ? "updated" : "created")} successfully";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = e.Message, IsSuccess = false });

            }
        }

        #endregion [ ADD / EDIT ]


        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsCreate)]
        [HttpGet]
        public IActionResult StatusConfimations(int id)
        {
            return PartialView("_StatusConfirmation", new Modal
            {
                Message = "Are you sure to Update this Banner?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Update Banner " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        #region [ UPDATE STATUS - Banner]      
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id , string imgfornamne)
        {
            try
            {
                StaticContentBanner staticContentBanner=new StaticContentBanner();
                int imgid = Enum.GetValues(typeof(ImageFor)).Cast<ImageFor>().Where(a => (a.ToString()) == imgfornamne).Select(a => (int)a).FirstOrDefault();
                var staticContentBannerObj = _staticContentBannerService.GetListStaticContentBannerById(imgid);
                if (staticContentBannerObj?.Count>0)
                {
                    for (int i = 0; i < staticContentBannerObj.Count; i++)
                    {
                        staticContentBannerObj[i].IsActive = false;
                        staticContentBannerObj[i].ModifiedDate = DateTime.UtcNow;
                        staticContentBannerObj[i].ModifiedBy = CurrentUser.Id;
                        await _staticContentBannerService.UpdateStaticContentBanner(staticContentBannerObj[i]);
                    }
                }
                var updated_data = _staticContentBannerService.GetStaticContentBannerById(Convert.ToInt32(id));
                if (updated_data == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                if (updated_data != null)
                {
                    updated_data.IsActive = !updated_data.IsActive;
                    updated_data.ModifiedDate = DateTime.UtcNow;
                    updated_data.ModifiedBy = CurrentUser.Id;
                    await _staticContentBannerService.UpdateStaticContentBanner(updated_data);
                }
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"Banner  {(staticContentBanner.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }
        }
        #endregion [ UPDATE STATUS ]



        #region [ DELETE Banner]
        [CustomAuthorization(AppPermissions.Pages_Administration_ManageBanner, AppPermissions.Action_IsDelete)]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Banner?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Banner " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _staticContentBannerService.GetStaticContentBannerById(Convert.ToInt32(id));

                if (Details != null)
                {
                    Details.IsDeleted = true;
                    await _staticContentBannerService.UpdateStaticContentBanner(Details);
                    ShowSuccessMessage("Success!", "Banner deleted successfully.", false);
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

        public ActionResult Thumbnail(int width, int height, string imageFile)
        {
            try
            {
                imageFile = Directory.GetCurrentDirectory() + "/wwwroot/" + SiteKeys.UploadFilesBanner + imageFile;
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
