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

namespace MVE.Admin.Controllers
{
    [Authorize]
    public class CountryController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IVisaGuideService _visaGuideService;
        public CountryController(ICountryService countryService, IVisaGuideService visaGuideService)
        {
            _countryService = countryService;
            _visaGuideService = visaGuideService;
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsRead)]
        public IActionResult Index()
        {
            CountryViewModel vm = new CountryViewModel();

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(MVE.DataTable.DataTables.DataTable dataTable, bool? status, string? fStartDate, string? fEndDate)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            List<int> column1 = new List<int>();
            for (int i = dataTable.iDisplayStart; i < dataTable.iDisplayStart + dataTable.iDisplayLength; i++)
            {
                column1.Add(i);
            }
            var query = new SearchQuery<CountryMaster>();
            if (status != null)
                query.AddFilter(b => b.IsActive == status);

            //if (!string.IsNullOrEmpty(fStartDate) && !string.IsNullOrEmpty(fEndDate))
            //{                
            //    query.AddFilter(ad => ad.CreatedOn <= ParsestringDateintoDatetime(fStartDate) && ad.CreatedOn >= ParsestringDateintoDatetime(fEndDate));
            //}
            //else if (!string.IsNullOrEmpty(fStartDate))
            //{
            //    query.AddFilter(ad => ad.CreatedOn == Convert.ToDateTime(ParsestringDateintoDatetime(fStartDate).ToString("yyyy-MM-dd"))); //&& ParsestringDateintoDatetime(fStartDate) == ad.CreatedOn.Date.AddDays(-1));
            //}
            //else if (!string.IsNullOrEmpty(fEndDate))
            //{
            //    query.AddFilter(ad => ad.CreatedOn.Date.AddDays(1) == ParsestringDateintoDatetime(fEndDate) && ParsestringDateintoDatetime(fEndDate) == ad.CreatedOn);
            //}
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
                    query.AddSortCriteria(new ExpressionSortCriteria<CountryMaster, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<CountryMaster, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<CountryMaster, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<CountryMaster, DateTime>(q => q.CreatedOn, SortDirection.Descending));
                    break;
            }
            query.Take = dataTable.iDisplayLength;
            query.Skip = dataTable.iDisplayStart;

            int count = dataTable.iDisplayStart + 1, total = 0;
           
            IEnumerable<CountryMaster> countrysobj = _countryService.Get(query, out total).Entities;

            foreach (CountryMaster r in countrysobj)
            {
                table.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    r.Id.ToString(),
                     count.ToString(),
                     r.Image??"", // string.IsNullOrEmpty(r.Image)?"No Image":r.Image,
                      r.Name,
                   //// r.Description??string.Empty,
                    r.CreatedOn.ToString(SiteKeys.DateFormatWithoutTime),
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

        #region [ ADD / EDIT Country]

        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsCreate)]

        [HttpGet]
        public IActionResult AddEditCountry(int? id)
        {
            CountryViewModel model = new CountryViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _countryService.GetCountryMasterById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.Code = accObj.Code;
                    model.IsActive = accObj.IsActive ? accObj.IsActive : false;
                    model.FileName = (string.IsNullOrEmpty(accObj.Image) ? "" : SiteKeys.UploadFilesCountry + accObj.Image);
                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_AddEditCountry", model);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult ViewCountry(int? id)
        {
            CountryViewModel model = new CountryViewModel();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var accObj = _countryService.GetCountryMasterById(id.Value);
                if (accObj != null)
                {
                    model.Id = accObj.Id;
                    model.Name = accObj.Name;
                    model.Description = accObj.Description;
                    model.Code = accObj.Code;
                    model.FileName = (string.IsNullOrEmpty(accObj.Image) ? "" : SiteKeys.UploadFilesCountry + accObj.Image);
                    model.FlagImage = new FormFile(null, 0, 0, SiteKeys.UploadFilesCountry + accObj.Image, accObj.Image);
                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_ViewCountry", model);
        }


        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult ViewCountryVisaGuid(int? id)
        {
            VisaGuideViewModels model = new VisaGuideViewModels();
            model.Id = id ?? 0;
            if (id.HasValue)
            {
                var visaguidDtls = _visaGuideService.GetVisaGuidyCountry(id ?? 0);
                if (visaguidDtls != null)
                {
                    model.Id = visaguidDtls.Id;
                    model.PageTitle = visaguidDtls.PageTitle;
                    model.ContentData = visaguidDtls.ContentData;
                    model.IsActive = visaguidDtls.IsActive ?? false;
                    model.AddedDate = visaguidDtls.CreatedOn;
                }
            }
            else
            {
                model.IsActive = true;
            }
            return PartialView("_ViewCountryVisaGuid", model);
        }


        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> AddEditCountry(CountryViewModel model)
        {
            try
            {
                CountryMaster country = null; bool isUpdate = false;
                if (model.Name != null && model.Id == 0)
                {
                    var isNameExist = IsNameExist(model);
                    if (isNameExist)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Country '" + model.Name + "' is already exists.", IsSuccess = false });
                    }
                }
                if (model.Id > 0)
                {
                    country = _countryService.GetCountryMasterById(model.Id);
                    if (country != null)
                    {
                        isUpdate = true;
                        country.ModifiedOn = DateTime.UtcNow;
                        country.ModifiedBy = CurrentUser.Id;
                    }
                }
                else
                {
                    country = new CountryMaster();
                    country.CreatedOn = DateTime.UtcNow;
                    country.CreatedBy = CurrentUser.Id;
                }
                if (model.FlagImage?.FileName != null)
                {
                    CommonFileViewModel.FileUpload(model.FlagImage, SiteKeys.UploadFilesCountry);
                    country.Image = model.FlagImage.FileName;

                }
                country.Name = model.Name;
                country.Description = model.Description;
                country.Code = (model.Code ?? "").ToUpper();
                country.Icon = "";

                country.IsActive = model.IsActive;

                country.ShortUrl = model.Name?.Replace(" ", "_").ToLower();


                var isshortNameExist = IsShortNameExist(country.ShortUrl, model.Id);
                if (isshortNameExist)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Country short url is already exists.", IsSuccess = false });
                }


                country = isUpdate ? await _countryService.UpdateCountryMaster(country) : await _countryService.SaveCountryMaster(country);


                string displayMsg = $"Country has been  {(isUpdate ? "updated" : "created")} successfully";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = e.Message, IsSuccess = false });

            }
        }

        #endregion [ ADD / EDIT ]
        #region [ EXISTS ]

        [HttpPost]
        public JsonResult IsCountryMasterExists(string Name, string PreInitialName)
        {
            bool isExist = false;
            if (Name != PreInitialName)
            {
                isExist = _countryService.IsCountryMasterExists(Name);
            }
            return Json(!isExist);
        }
        public bool IsNameExist(CountryViewModel model)
        {
            bool isExist = false;

            isExist = _countryService.IsCountryMasterExists(model.Name);

            return isExist;
        }

        public bool IsShortNameExist(string _ShortName, int ids)
        {
            bool isExist = false;

            isExist = _countryService.IsCountryShortNameExists(_ShortName, ids);

            return isExist;
        }

        #endregion [ EXISTS ]

        #region [ UPDATE STATUS - Country]   
        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            try
            {
                var countryObj = _countryService.GetCountryMasterById(Convert.ToInt32(id));
                if (countryObj == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Record not found", IsSuccess = false });
                }
                countryObj.IsActive = !countryObj.IsActive;
                countryObj.ModifiedOn = DateTime.UtcNow;
                countryObj.ModifiedBy = CurrentUser.Id;
                await _countryService.UpdateCountryMaster(countryObj);

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = $"Country  {(countryObj.IsActive == true ? "Active" : "Inactive")} successfully.", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.GetBaseException().Message, IsSuccess = false });
            }
        }
        #endregion [ UPDATE STATUS ]

        #region [ DELETE Country]

        [HttpGet]
        [CustomAuthorization(AppPermissions.Pages_Administration_Country, AppPermissions.Action_IsDelete)]
        public IActionResult Delete(int id)
        {
            return PartialView("_ModalDelete", new Modal
            {
                Message = "Are you sure to delete this Country?",
                Size = ModalSize.Small,
                Header = new ModalHeader { Heading = "Delete Country " },
                Footer = new ModalFooter { SubmitButtonText = "Yes", CancelButtonText = "No" }

            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, IFormCollection FC)
        {
            try
            {
                var Details = _countryService.GetCountryMasterById(id);

                if (Details != null)
                {
                    Details.IsDeleted = true;
                    await _countryService.UpdateCountryMaster(Details);
                    ShowSuccessMessage("Success!", "Country deleted successfully.", false);
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
                imageFile = Directory.GetCurrentDirectory() + "/wwwroot/" + SiteKeys.UploadFilesCountry + imageFile;
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
