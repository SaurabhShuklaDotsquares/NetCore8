using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.Attributes;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.Dto;
using MVE.Service;

namespace MVE.Admin.Controllers
{
    public class SettingsController : BaseController
    {
        private readonly ISettingService _settingService;
        private readonly IAdminUserService _adminUserService;
        public SettingsController(ISettingService settingService, IAdminUserService adminUserService)
        {
            _settingService = settingService;
            _adminUserService = adminUserService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region [Manage]

        [HttpGet]
        [CustomAuthorization(AppPermissions.Pages_Administration_Settings, AppPermissions.Action_IsRead)]
        public IActionResult Manage(int? id)
        {
            GeneralSiteSettingViewModel model = new GeneralSiteSettingViewModel();
            model.Id = id ?? 0;
            var accObj = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
            if (accObj != null)
            {
                model.Id = accObj.Id;
                model.SiteName = accObj.SiteName;
                model.SupportEmail = accObj.SupportEmail;
                model.SupportMobile = accObj.SupportMobile;
                model.Address = accObj.Address;
                model.AdminPageLimit = accObj.AdminPageLimit;
                model.ApplyTaxPercentHeading1 = accObj.ApplyTaxPercentHeading1;
                model.ApplyTaxPercent1 = accObj.ApplyTaxPercent1 != null ? Convert.ToInt32(accObj.ApplyTaxPercent1.Value) : 0;
                model.ApplyTaxPercentHeading2 = accObj.ApplyTaxPercentHeading2;                
                model.ApplyTaxPercent2 = accObj.ApplyTaxPercent2 != null ? Convert.ToInt32(accObj.ApplyTaxPercent2.Value) : 0; ;                
                model.DiscountPercent = accObj.DiscountPercent != null ? Convert.ToInt32(accObj.DiscountPercent.Value) : 0; ;
                model.IsApplyDiscountPercent = accObj.IsApplyDiscountPercent != null ? accObj.IsApplyDiscountPercent.Value : false;                
                model.DiscountFix = accObj.DiscountFix != null ? Convert.ToInt32(accObj.DiscountFix.Value) : 0; ;
                model.IsApplyDiscountFix = accObj.IsApplyDiscountFix != null ? accObj.IsApplyDiscountFix.Value : false;

                model.LogoImageName = (string.IsNullOrEmpty(accObj.LogoImageName) ? "" : SiteKeys.UploadFilesTheme + accObj.LogoImageName);
                model.File = new FormFile(null, 0, 0, SiteKeys.UploadFilesTheme + accObj.LogoImageName, accObj.LogoImageName);

                model.LogoImageNameDark = (string.IsNullOrEmpty(accObj.LogoImageNameDark) ? "" : SiteKeys.UploadFilesTheme + accObj.LogoImageNameDark);
                model.FileDark = new FormFile(null, 0, 0, SiteKeys.UploadFilesTheme + accObj.LogoImageNameDark, accObj.LogoImageNameDark);
            }
            return View(model);
        }


        [HttpPost]
        [CustomAuthorization(AppPermissions.Pages_Administration_Settings, AppPermissions.Action_IsCreate)]
        public async Task<IActionResult> Manage(GeneralSiteSettingViewModel model)
        {
            try
            {
                GeneralSiteSetting generalSiteSetting = null; bool isUpdate = false;                
                if (model.Id > 0)
                {
                    generalSiteSetting = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
                    if (generalSiteSetting != null)
                    {
                        isUpdate = true;
                        generalSiteSetting.ModifiedOn = DateTime.UtcNow;
                        generalSiteSetting.ModifiedBy = CurrentUser.Id;
                    }
                }
                else
                {
                    generalSiteSetting = new GeneralSiteSetting();
                    generalSiteSetting.IsActive = true;
                    generalSiteSetting.CreatedOn = DateTime.UtcNow;
                    generalSiteSetting.CreatedBy = CurrentUser.Id;
                }
                if (model.File?.FileName != null)
                {
                    CommonFileViewModel.FileUpload(model.File, SiteKeys.UploadFilesTheme);
                    generalSiteSetting.LogoImageName = model.File.FileName;

                }
                if (model.FileDark?.FileName != null)
                {
                    CommonFileViewModel.FileUpload(model.FileDark, SiteKeys.UploadFilesTheme);
                    generalSiteSetting.LogoImageNameDark = model.FileDark.FileName;

                }
                generalSiteSetting.SiteName = model.SiteName;
                generalSiteSetting.SupportEmail = model.SupportEmail;
                generalSiteSetting.SupportMobile = model.SupportMobile;
                generalSiteSetting.Address = model.Address;
                generalSiteSetting.AdminPageLimit = model.AdminPageLimit;

                generalSiteSetting.ApplyTaxPercentHeading1 = model.ApplyTaxPercentHeading1;
                generalSiteSetting.ApplyTaxPercent1 = model.ApplyTaxPercent1;
                generalSiteSetting.ApplyTaxPercentHeading2 = model.ApplyTaxPercentHeading2;
                generalSiteSetting.ApplyTaxPercent2 = model.ApplyTaxPercent2;
                generalSiteSetting.DiscountPercent = model.DiscountPercent;
                generalSiteSetting.IsApplyDiscountPercent = model.IsApplyDiscountPercent;
                generalSiteSetting.DiscountFix = model.DiscountFix;
                generalSiteSetting.IsApplyDiscountFix = model.IsApplyDiscountFix;


                generalSiteSetting = isUpdate ? await _settingService.UpdateGeneralSiteSettings(generalSiteSetting) : await _settingService.SaveGeneralSiteSettings(generalSiteSetting);
                string displayMsg = $"Setting has been  {(isUpdate ? "updated" : "created")} successfully";
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg });
                //ShowSuccessMessage("Success!", displayMsg, false);
                //return RedirectToAction("index");
            }
            catch (Exception e)
            {
                if (e.Message.ToString() == "Parameter is not valid.")
                {
                    ShowErrorMessage("Error!", "Image is not valid.", false);
                    return RedirectToAction("manage", "package", new { Id = model.Id });
                }
                else
                {
                    ShowErrorMessage("Error!", "Oops! Something went wrong, please refresh the page and try again.", false);
                    return RedirectToAction("index");
                }
            }
        }


        #endregion [ Manage ]

        [HttpGet]
        public IActionResult AdminPersonalDetails()
        {
            AdminPersonalDetailsViewModel model = new AdminPersonalDetailsViewModel();
          

            var obj = _adminUserService.GetAdminUserById(CurrentUser.Id);
            if (obj != null)
            {
                model.Id = obj.Id;
                model.FullName = CommonFileViewModel.GetFullName(obj.FirstName, obj.LastName);
                model.Email = obj.Email;
                model.MobileNo = obj.MobilePhone ?? "NA";
                model.Address = obj.Address ?? "NA";
                model.Description = obj.Description??"NA";
                model.LastUpdateDate = obj.ModifiedOn != null ? obj.ModifiedOn.ToString("dd/MM/yyyy") : "";

            }
            return View(model);
        }
    }
}
