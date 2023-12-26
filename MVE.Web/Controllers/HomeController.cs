using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TCP.Core.Code.Extensions;
using TCP.Core.Code.LIBS;
using TCP.Data.Models;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using TCP.Dto;
using TCP.Service;
using TCP.Web.Models;
using TCP.Web.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Web;
using TCP.Core.Code;
using System.Drawing;
using Newtonsoft.Json;
using static TCP.Web.ViewModels.CommonFileViewModel;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using TCP.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using TCP.Web.ViewModels.VisaGuide;
using TCP.Service.Banner;

namespace TCP.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IThemeService _themeService;
        private readonly IPackageService _packageService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly TravelCustomPackagesContext _context;
        private readonly IDestinationService _destinationService;
        private readonly ICustomQuoteService _customQuoteService;
        private readonly ICountryService _countryService;
        private readonly ISettingService _settingService;
        private readonly IStaticContentBannerService _staticContentBannerService;

      

        public HomeController(ILogger<HomeController> logger, IThemeService themeService, IHostingEnvironment hostingEnvironment
            , IPackageService packageService, TravelCustomPackagesContext context, IDestinationService destinationService, ICustomQuoteService customQuoteService, ICountryService countryService, ISettingService settingService, IStaticContentBannerService staticContentBannerService)
        {
            _logger = logger;
            _themeService = themeService;
            _packageService = packageService;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _destinationService = destinationService;
            _customQuoteService = customQuoteService;
            _countryService = countryService;
            _settingService = settingService;
            _staticContentBannerService= staticContentBannerService;
        }

        [HttpGet]
        public IActionResult Index(int? budget, string? destination, int? duration, int? reqid)
        {
            //if (CurrentFrontUser.Id > 0)
            //{
            //    return RedirectToAction("Index", "Dashboard");
            //}
            IndexViewModel vmodel = new IndexViewModel();
            List<ThemeViewModel> themeList = new List<ThemeViewModel>();
            List<PackageListViewModel> pkgList = new List<PackageListViewModel>();

            vmodel.BudgetList = GetBudgetLists(budget ?? 3);
            vmodel.DurationList = GetDurationLists(duration ?? 3);
            #region Banner List

            var Imagedata = _staticContentBannerService.GetStaticContentBanner();
            if (Imagedata!=null)
            {
                vmodel.Imagename = Imagedata.ImageName;
            }

            #endregion

            #region Thems Data Binding
            var thems = _themeService.GetAllThemes(true, false);
            foreach (var item in thems)
            {
                ThemeViewModel themeView = new ThemeViewModel();
                themeView.Name = item.Name;
                themeView.ImageName = item.ImageName;
                themeView.Description = item.Description;
                themeView.Id = item.Id;
                bool fileExists = CheckIfFileExists(SiteKeys.UploadFilesTheme, item.ImageName);
                themeView.FileName = fileExists == false ? SiteKeys.NoImagePath : SiteKeys.UploadFilesTheme + item.ImageName;
                themeView.ShortName = item.ShortName;
                themeList.Add(themeView);
            }
            vmodel.ThemeList = themeList;
            #endregion

            //var packages = _packageService.GetPackagesList().Where(x => x.IsPublish).ToList();
            var packages = _packageService.GetPackagesListByFutureDate();
            if (budget > 0)
            {
                packages = BudgetFilter(budget, packages);
            }
            if (duration > 0)
            {
                packages = DurationFilter(duration, packages);
            }
            vmodel.PackagesList = BindPackageListData(packages, pkgList);
            vmodel.RegionList = _context.Regions.Select(c => new EnumsList() { Id = c.Id, Description = c.Description, Name = $"{c.Name}" }).ToList();

            vmodel.DestinationList = _countryService.GetCountryMastersForDropDownWithShortURL().OrderBy(o => o.Text).ToList();
                       
            return View(vmodel);
        }
           

        private List<PackageListViewModel> BindPackageListData(List<Package> packages, List<PackageListViewModel> pkgList)
        {
            foreach (var item in packages)
            {

                PackageListViewModel pkgObj = new PackageListViewModel();
                pkgObj.PackageId = item.Id;
                pkgObj.LocationAddress = item.Country?.Name ?? "-";
                pkgObj.LocationId = item.Country?.Id;
                pkgObj.PkgDtlId = item.Id;
                pkgObj.IsCruseIncluded = item.IsCruseIncluded;
                pkgObj.PackagePrice = item.PackagePrice;
                pkgObj.PackagePriceFront = item.PackagePrice.ToString("0.##");
                pkgObj.PackageName = item.Name;
                pkgObj.PackageUrl = item.PackageUrl;
                ////pkgObj.PkgDesc = item.Description?.Length <= 83 ? item.Description : item.Description?.Substring(0, 83) + "..."; ;
                pkgObj.PkgDesc = item.Description?.Length <= 83 ? item.Description : TrimHtmlText(item.Description, 80);
                pkgObj.PackageNoOf_DaysNight = item.PackageNoOfDays.ToString() + "Days & " + item.PackageNoOfNights.ToString() + "Nights";
                var imagesObj = item.PackageImages?.FirstOrDefault();
                if (imagesObj != null)
                {
                    pkgObj.FileOriginalName = imagesObj.OriginalImageName ?? "";
                    pkgObj.FileExtension = imagesObj.ImageExtension;
                    pkgObj.PkgImgId = imagesObj.Id;


                    //pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin((UploadSection)imagesObj.ImageSection, imagesObj.ImageName, imagesObj.ImageExtension, imagesObj.OriginalImageName);
                    string RetImageName = "";
                    pkgObj.FilePath = GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);
                    imagesObj.ImageName = RetImageName;
                    //string filePath = string.Empty;
                    //string ImageName = string.Empty;
                    //try
                    //{
                    //    pkgObj.FilePath = "";
                    //    //UploadSection uploadSection;
                    //    UploadSection us = (UploadSection)imagesObj.ImageSection;
                    //    if (!string.IsNullOrWhiteSpace(imagesObj.ImageName))
                    //    {
                    //        string datetimeString = imagesObj.ImageName.Replace($"{Path.GetFileNameWithoutExtension(imagesObj.OriginalImageName)}_", "");
                    //        datetimeString = Path.GetFileNameWithoutExtension(datetimeString);
                    //        DateTime fileCreatedDateTime = new DateTime(Convert.ToInt64(datetimeString));
                    //        pkgObj.FilePath = string.Concat(((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty),
                    //                                       "/", SiteKeys.UploadDirectory, "/",
                    //                                        us.GetEnumDescription(), "/",
                    //                                        fileCreatedDateTime.Year, "/",
                    //                                        fileCreatedDateTime.Month);

                    //        if (!string.IsNullOrWhiteSpace(pkgObj.FilePath))
                    //        {
                    //            filePath = string.Concat(pkgObj.FilePath, "/", imagesObj.ImageName, imagesObj.ImageExtension);
                    //            if (filePath != null)
                    //            {
                    //                filePath = SiteKeys.ImageDomain + "/" + filePath;
                    //                pkgObj.FilePath = filePath;
                    //            }
                    //        }
                    //    }
                    //}
                    //catch (Exception ex)
                    //{

                    //}

                    //bool fileExists = CheckIfFileExists(pkgObj.FilePath, "");
                    //pkgObj.ImageName = fileExists == false ? SiteKeys.NoImagePath : SiteKeys.UploadFilesTheme + pkgObj.FileOriginalName;
                }
                else
                {
                    pkgObj.FilePath = SiteKeys.NoImagePath_Square;
                }
                pkgList.Add(pkgObj);
            }
            return pkgList;
        }

        private bool CheckIfFileExists(string filepath, string imageName)
        {
            try
            {
                // string filePath = Path.Combine(_hostingEnvironment.WebRootPath + filepath + imageName);
                string filePath = Path.Combine((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty + filepath + imageName);
                //return System.IO.File.Exists(filePath);
                if (filePath != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<Package> DurationFilter(int? duration, List<Package> packages)
        {
            switch (duration)
            {
                case (int)PricedPkgInDuration.Days1to3:
                    packages = packages.Where(x => x.PackageNoOfDays >= 1 && x.PackageNoOfDays <= 3).ToList();
                    break;
                case (int)PricedPkgInDuration.Days4to6:
                    packages = packages.Where(x => x.PackageNoOfDays >= 4 && x.PackageNoOfDays <= 6).ToList();/////
                    break;
                case (int)PricedPkgInDuration.Days7to9:
                    packages = packages.Where(x => x.PackageNoOfDays >= 7 && x.PackageNoOfDays <= 9).ToList();
                    break;
                case (int)PricedPkgInDuration.Days10to12:
                    packages = packages.Where(x => x.PackageNoOfDays >= 10 && x.PackageNoOfDays <= 12).ToList();
                    break;
                case (int)PricedPkgInDuration.GreaterThan13days:
                    packages = packages.Where(x => x.PackageNoOfDays > 13).ToList();
                    break;
                default:
                    packages = packages.Where(x => x.PackageNoOfDays >= 7 && x.PackageNoOfDays <= 9).ToList();
                    break;
            }
            return packages;

        }
        private List<Package> BudgetFilter(int? budget, List<Package> packages)
        {
            switch (budget)
            {
                case (int)PricedPkgInBudget.LessThan1K:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) <= 1000).ToList();
                    break;
                case (int)PricedPkgInBudget.From2Kto4K:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) >= 2000 && Math.Floor(x.PackagePrice) <= 4000).ToList();
                    break;
                case (int)PricedPkgInBudget.From4Kto6K:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) >= 4000 && Math.Floor(x.PackagePrice) <= 6000).ToList();
                    break;
                case (int)PricedPkgInBudget.From6Kto8K:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) >= 6000 && Math.Floor(x.PackagePrice) <= 8000).ToList();
                    break;
                case (int)PricedPkgInBudget.GreaterThan9K:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) >= 9000).ToList();
                    break;
                default:
                    packages = packages.Where(x => Math.Floor(x.PackagePrice) >= 4000 && Math.Floor(x.PackagePrice) <= 6000).ToList();
                    break;
            }
            return packages;

        }

        private List<EnumsList> GetBudgetLists(int? activebudget)
        {
            List<EnumsList> lst = Enum.GetValues(typeof(PricedPkgInBudget))
                               .Cast<PricedPkgInBudget>()
                               .Select(g => new EnumsList
                               {
                                   Id = Convert.ToInt16(g),
                                   Name = g.GetDescription(),
                                   Active = Convert.ToInt16(g) == activebudget ? "active" : ""
                               })
                               .ToList();
            return lst;

        }

        private List<EnumsList> GetDurationLists(int? activeduration)
        {
            List<EnumsList> lst = Enum.GetValues(typeof(PricedPkgInDuration))
                               .Cast<PricedPkgInDuration>()
                               .Select(g => new EnumsList
                               {
                                   Id = Convert.ToInt16(g),
                                   Name = g.GetDescription(),
                                   Active = Convert.ToInt16(g) == activeduration ? "active" : ""
                               })
                               .ToList();
            return lst;

        }

        public IActionResult ThemeList(List<ThemeViewModel> model)
        {
            return PartialView("_ThemeList", model);
        }
        [HttpGet]
        public IActionResult PackageList(List<PackageListViewModel> model)
        {

            return PartialView("_PackageList", model);
        }
        [HttpPost]
        public IActionResult PackageList(int? budget, string? destination, int? duration)
        {
            List<PackageListViewModel> models = new List<PackageListViewModel>();
            //var packages = _packageService.GetPackagesList().Where(x => x.IsPublish).ToList();
            var packages = _packageService.GetPackagesListByFutureDate();
            if (budget > 0)
            {
                packages = BudgetFilter(budget, packages);
            }
            if (duration > 0)
            {
                packages = DurationFilter(duration, packages);
            }
            models = BindPackageListData(packages, models);

            return PartialView("_PackageList", models);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public JsonResult GetThemeViewData()
        {
            var themes = _themeService.GetAllThemes(true, false);
            List<CustomThemeViewModel> tvmL = null;
            CustomThemeViewModel tvm = null;
            if (themes.ToList().Count > 0)
            {
                tvmL = new List<CustomThemeViewModel>();
                foreach (var key in themes)
                {
                    tvm = new CustomThemeViewModel();
                    tvm.Id = key.Id;
                    tvm.Name = key.Name;
                    tvm.ShortName = key.ShortName;
                    tvm.Description = SplitString(key.Description, 30);
                    tvm.ImageName = key.ImageName;
                    tvmL.Add(tvm);
                }
            }


            //var html=JsonConvert.SerializeObject(themes);

            //return Json(themes, JsonRequestBehavior.AllowGet);
            //var data = Newtonsoft.Json.JsonConvert.SerializeObject(themes);

            return Json(tvmL);

        }
        #region [ Front User - Home Contact Us ]
        // contact us get
        // contact us post is same as dashboard's contactus... so we are using dashboard's post method for saving.
        [HttpGet]
        public IActionResult HomeContactUs(string? openFrom)
        {
            ViewBag.OpenFrom = openFrom;
            ContactUsViewModel usViewModel = new ContactUsViewModel();
            var setting = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
            usViewModel.SupportEmail = setting.SupportEmail ?? "";
            usViewModel.SupportMobile = setting.SupportMobile ?? "";

            var globalSetting = GetGlobalSetting();
            usViewModel.SupportEmail = globalSetting.SupportEmail;
            usViewModel.SupportMobile = globalSetting.SupportMobile;
            usViewModel.LogoImageName = globalSetting.LogoImageName;
            usViewModel.LogoImageNameDark = globalSetting.LogoImageNameDark;

            return PartialView("_HomeContactUs", usViewModel);
        }
        [HttpGet]
        public IActionResult HomeDestinationGuide(string? openFrom)
        {
            ViewBag.OpenFrom = openFrom;
            DestinationGuideViewModel usViewModel = new DestinationGuideViewModel();
            List<CountryViewModel> countryViews = new List<CountryViewModel>();
            var Countrylst =_countryService.GetAllCountriesWithoutStateMatch(true,false);
            foreach (var item in Countrylst)
            {
                CountryViewModel country = new CountryViewModel();
                country.Id= item.Id;
                country.Name= item.Name;
                country.Description = item.Description;
                country.ShortUrl = item.ShortUrl;
                if (!string.IsNullOrEmpty(item.Image))
                {
                    bool fileExists1 = CommonFileViewModel.CheckIfFileExists(SiteKeys.UploadFilesCountry, item.Image ?? "");
                    var fullFileName1 = fileExists1 == false ? SiteKeys.NoImagePath_Square : SiteKeys.ImageDomain + "/" + SiteKeys.UploadFilesCountry + item.Image;
                    country.FileName = fullFileName1;

                }
                else
                {
                    country.FileName = SiteKeys.NoImagePath_Square;
                }
                countryViews.Add(country);  
            }
            usViewModel.CountryMasterslst = countryViews;
            return PartialView("_HomeDestinationGuide", usViewModel);
        }


        [HttpGet]
        public IActionResult HomeVisaGuide(string? openFrom)
        {
            ViewBag.OpenFrom = openFrom;
            VisaGuideViewModel usViewModel = new VisaGuideViewModel();
            List<CountryViewModel> countryViews = new List<CountryViewModel>();
            var Countrylst = _countryService.GetAllCountriesWithoutStateMatch(true, false);
            foreach (var item in Countrylst)
            {
                CountryViewModel country = new CountryViewModel();
                country.Id = item.Id;
                country.Name = item.Name;
                country.Description = item.Description;
                country.ShortUrl = item.ShortUrl;
                if (!string.IsNullOrEmpty(item.Image))
                {
                    bool fileExists1 = CommonFileViewModel.CheckIfFileExists(SiteKeys.UploadFilesCountry, item.Image ?? "");
                    var fullFileName1 = fileExists1 == false ? SiteKeys.NoImagePath_Square : SiteKeys.ImageDomain + "/" + SiteKeys.UploadFilesCountry + item.Image;
                    country.FileName = fullFileName1;

                }
                else
                {
                    country.FileName = SiteKeys.NoImagePath_Square;
                }
                countryViews.Add(country);
            }
            usViewModel.CountryMasterslst = countryViews;
            return PartialView("_HomeVisaGuide", usViewModel);
        }

        private GeneralSiteSettingDTO GetGlobalSetting()
        {
            var globalSetting = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
            GeneralSiteSettingDTO obj = new GeneralSiteSettingDTO();
            if (globalSetting != null)
            {
                obj.SupportEmail = globalSetting.SupportEmail;
                obj.SupportMobile = globalSetting.SupportMobile;
                obj.LogoImageName = SiteKeys.ImageDomain + SiteKeys.UploadFilesTheme + globalSetting.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.ImageDomain + SiteKeys.UploadFilesTheme + globalSetting.LogoImageNameDark;
            }
            else
            {
                obj.SupportEmail = SiteKeys.SupportEmail;
                obj.SupportMobile = SiteKeys.SupportMobile;
                obj.LogoImageName = SiteKeys.Domain+ "/Image"+ SiteKeys.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.Domain + "/Image" + SiteKeys.LogoImageNameDark;
            }
            return obj;
        }


        [HttpGet]
        public JsonResult GetGeneralSieSettings()
        {
            var globalSetting = GetGlobalSetting();           
            return Json(globalSetting);
        }


        #endregion
    }
}