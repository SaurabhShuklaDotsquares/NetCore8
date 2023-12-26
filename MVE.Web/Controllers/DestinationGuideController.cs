using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCP.Core.Code.LIBS;
using TCP.Service;
using TCP.Web.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TCP.Web.Controllers
{
    public class DestinationGuideController : BaseController
    {
        private readonly IManageDestinationService _ManageDestService;
        private readonly ICountryService _countryService;
        public DestinationGuideController(IManageDestinationService ManageDestService, ICountryService countryService)
        {

            _ManageDestService = ManageDestService;
            _countryService = countryService;
        }


        public IActionResult Index(string country)
        {
            DestinationGuideViewModel vm = new DestinationGuideViewModel();
            var dtlsByShortURL = _countryService.GetCountrybyShortName(country);
            if (dtlsByShortURL==null)
            {
                return View(null);
            }
            vm.CountryId = dtlsByShortURL.Id;
            var destHeadDtls = _ManageDestService.GetDestinationHeaderByCountry(vm.CountryId);
            vm.CountryName = dtlsByShortURL.Name;
           // ViewBag.CountryName = vm.CountryName;
            if (destHeadDtls != null)
            {
                List<string> lstImages = new List<string>();
                for (int i = 0; i < destHeadDtls.Count; i++)
                {
                    if (i == 0)
                    {
                        vm.HeadingTitle = destHeadDtls[i].HeadingTitle;
                        vm.HeadingContent = destHeadDtls[i].HeadingContent;                       
                    }
                    if (!string.IsNullOrEmpty(destHeadDtls[i].ImageName))
                    {
                        bool fileExists = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destHeadDtls[i].ImageName??"");
                        var fullFileName = fileExists == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destHeadDtls[i].ImageName;
                        lstImages.Add(fullFileName);
                    }
                    else
                    {
                        //var fullFileName = SiteKeys.NoImagePath_Square;
                        lstImages.Add("");
                    }
                }
                vm.lstImageNames = lstImages;

            }


            var destMidFootDtls = _ManageDestService.GetDestinationMidFootByCountry(vm.CountryId);
            if (destMidFootDtls != null)
            {
                vm.DestMidFootId = destMidFootDtls.Id;
                vm.MiddleTitle = destMidFootDtls.MiddleTitle;
                vm.MiddleContent = destMidFootDtls.MiddleContent;
                if (!string.IsNullOrEmpty(destMidFootDtls.MiddleImageNameMain))
                {
                    bool fileExists = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.MiddleImageNameMain ?? "");
                    var fullFileName = fileExists == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.MiddleImageNameMain;
                    vm.MiddleImageNameMain = fullFileName;
                }
                if (!string.IsNullOrEmpty(destMidFootDtls.MiddleImageName1))
                {
                    bool fileExists1 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.MiddleImageName1 ?? "");
                    var fullFileName1 = fileExists1 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.MiddleImageName1;
                    vm.MiddleImageName1 = fullFileName1;

                }
                if (!string.IsNullOrEmpty(destMidFootDtls.MiddleImageName2))
                {
                    bool fileExists2 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.MiddleImageName2 ?? "");
                    var fullFileName2 = fileExists2 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.MiddleImageName2;
                    vm.MiddleImageName2 = fullFileName2;
                }
                if (!string.IsNullOrEmpty(destMidFootDtls.MiddleImageName3))
                {
                    bool fileExists3 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.MiddleImageName3 ?? "");
                    var fullFileName3 = fileExists3 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.MiddleImageName3;
                    vm.MiddleImageName3 = fullFileName3;
                }

                if (!string.IsNullOrEmpty(destMidFootDtls.MiddleImageName4))
                {
                    bool fileExists4 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.MiddleImageName4 ?? "");
                    var fullFileName4 = fileExists4 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.MiddleImageName4;
                    vm.MiddleImageName4 = fullFileName4;
                }

                vm.FooterTitle = destMidFootDtls.FooterTitle;
                vm.FooterContent = destMidFootDtls.FooterContent;
                vm.FooterBottomContent = destMidFootDtls.FooterBottomContent;
                if (!string.IsNullOrEmpty(destMidFootDtls.FooterImageName1))
                {
                    bool fileExistsfoot1 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.FooterImageName1 ?? "");
                    var fullFileNamefoot1 = fileExistsfoot1 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.FooterImageName1;
                    vm.FooterImageName1 = fullFileNamefoot1;
                }
                if (!string.IsNullOrEmpty(destMidFootDtls.FooterImageName1))
                {
                    bool fileExistsfoot2 = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesDestination, destMidFootDtls.FooterImageName2 ?? "");
                    var fullFileNamefoot2 = fileExistsfoot2 == false ? SiteKeys.NoImagePath_Square : SiteKeys.UploadFilesDestination + destMidFootDtls.FooterImageName2;
                    vm.FooterImageName2 = fullFileNamefoot2;
                }
               
            }
            return View(vm);
        }


    }
}
