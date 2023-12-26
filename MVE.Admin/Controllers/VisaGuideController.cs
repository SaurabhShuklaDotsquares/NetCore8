using Microsoft.AspNetCore.Mvc;
using MVE.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVE.Core;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.Service;
using DocumentFormat.OpenXml.Office2010.Excel;
using MVE.Core.Code.Attributes;

namespace MVE.Admin.Controllers
{
    public class VisaGuideController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IVisaGuideService _visaGuideService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly ICountryService _countryService;

        public VisaGuideController(IUserService userService, IVisaGuideService visaGuideService, IEmailFactoryService emailFactoryService, ICountryService countryService)
        {
            _userService = userService;
            _visaGuideService = visaGuideService;
            _emailFactoryService = emailFactoryService;
            _countryService = countryService;
        }
        [CustomAuthorization(AppPermissions.Pages_Administration_VisaGuideManager, AppPermissions.Action_IsRead)]
        public IActionResult Index(int? id)
        {
            VisaGuideViewModels vm = new VisaGuideViewModels();
            vm.CountryId = Convert.ToInt32(id);
            ViewBag.CountryName = _countryService.GetCountryMasterById(Convert.ToInt32(id))?.Name;
            return View(vm);
        }

        [CustomAuthorization(AppPermissions.Pages_Administration_VisaGuideManager, AppPermissions.Action_IsRead)]
        [HttpGet]
        public IActionResult VisaGuideSection(int? id)
        {
            VisaGuideViewModels vm = new VisaGuideViewModels();
            vm.CountryId = id ?? 0;
            vm.IsActive = true;
            if (vm.CountryId > 0)
            {
                var visaguidDtls = _visaGuideService.GetVisaGuidyCountry(vm.CountryId ?? 0);
                if (visaguidDtls != null)
                {
                    vm.Id = visaguidDtls.Id;
                    vm.PageTitle = visaguidDtls.PageTitle;
                    vm.ContentData = visaguidDtls.ContentData;
                    vm.IsActive = visaguidDtls.IsActive ?? false;
                    vm.AddedDate = visaguidDtls.CreatedOn;
                }
            }
            return PartialView("_VisaGuideSection", vm);
        }

        // Visa guide Save/Edit
        [CustomAuthorization(AppPermissions.Pages_Administration_VisaGuideManager, AppPermissions.Action_IsCreate)]
        [HttpPost]
        public async Task<IActionResult> Index(VisaGuideViewModels vm)
        {
            try
            {
                VisaGuide visaguidDtls = new VisaGuide();
                int id = 0;
                var destHeadDtls = _visaGuideService.GetVisaGuidyCountry(vm.CountryId ?? 0);
                if (destHeadDtls != null)
                {
                    destHeadDtls.Id = destHeadDtls.Id;
                    destHeadDtls.CountryId = vm.CountryId ?? 0;
                    destHeadDtls.Name = "Visa Guide";
                    destHeadDtls.PageTitle = vm.PageTitle;
                    destHeadDtls.ContentData = vm.ContentData;
                    destHeadDtls.ModifiedOn = DateTime.UtcNow;
                    destHeadDtls.IsActive = vm.IsActive;
                    await _visaGuideService.UpdateVisaGuide(destHeadDtls);

                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Visa guide updated successfully.", IsSuccess = true, RedirectUrl = Url.Action("Index") });
                }
                else
                {
                    var counntryname = _countryService.GetCountryMasterById(Convert.ToInt32(vm.CountryId))?.Name;
                    // DestinationHeader destHead = new DestinationHeader();
                    visaguidDtls.Url = counntryname.Trim().ToLower().Replace(" ", "_");
                    visaguidDtls.Name = "Visa Guide";
                    visaguidDtls.PageTitle = vm.PageTitle;
                    visaguidDtls.CountryId = vm.CountryId ?? 0;
                    visaguidDtls.ContentData = vm.ContentData;
                    visaguidDtls.CreatedOn = DateTime.UtcNow;
                    visaguidDtls.IsActive = vm.IsActive;
                    await _visaGuideService.SaveVisaGuide(visaguidDtls);
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Visa guide saved successfully.", IsSuccess = true, RedirectUrl= Url.Action("Index") });
                }
            }
            catch (Exception e)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = e.Message, IsSuccess = false });

            }
        }

    }
}
