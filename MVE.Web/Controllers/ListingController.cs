using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TCP.Core.Code;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using TCP.Core.Code.LIBS;
using TCP.Data.Models;
using TCP.Service;
using TCP.Web.ViewModels;
using NuGet.Configuration;
using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using TCP.Dto;
using Newtonsoft.Json;
using System.Security.Claims;
//using TCP.Admin.Controllers;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.Xml;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Drawing.Printing;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCP.Web.Controllers
{
    public class ListingController : BaseController
    {
        //public IActionResult Index(int? budget, string? destination, int? duration, int? reqid)
        //{
        //    PackageViewModel pkgmodel = new PackageViewModel();
        //   // var packages = _pkgservice.GetPackagesList();

        //    return View(pkgmodel);
        //}
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly TravelCustomPackagesContext _context;
        private readonly IPackageService _pkgservice;
        private readonly IThemeService _themeService;
        private readonly ICountryService _countryService;
        private readonly IRegionService _regionService;

        public ListingController(IEmailFactoryService emailFactoryService,
            TravelCustomPackagesContext context, IPackageService pkgservice, IThemeService themeService, ICountryService countryService, IRegionService regionService)
        {
            _emailFactoryService = emailFactoryService;
            _context = context;
            _pkgservice = pkgservice;
            _themeService = themeService;
            _countryService = countryService;
            _regionService = regionService;
        }

        [HttpGet]
        public PartialViewResult GetProductListByFilter(string? budget, string? destination, string? duration, string? category, int? reqid, string? regionId, int? IsCruseIncluded, int? pkgid, int curentPage = 1, int pageSize = 6, string? searchType = "", string? durationVal = "", string inclusionVal = "", string ratingVal = "", string priceRange = "", string countryVal = "", string directionVal = "")
        {

            ListingViewModel model = new ListingViewModel();
            model = GetProductSearchResult(budget, destination, duration, category, reqid, regionId, IsCruseIncluded, pkgid, curentPage, pageSize, searchType, durationVal, inclusionVal, ratingVal, priceRange, countryVal, directionVal);
            return PartialView("~/Views/Listing/_ListingPackage.cshtml", model);
        }


        private ListingViewModel GetProductSearchResult(string? budget, string? destination, string? duration, string? category, int? reqid, string? regionId, int? IsCruseIncluded, int? pkgid, int curentPage = 1, int pageSize = 6, string? searchType = "", string? durationVal = "", string inclusionVal = "", string ratingVal = "", string priceRange = "", string countryVal = "", string directionVal = "")
        {
            int skipSize = 6;
            int? budget1 = 0;
            int? duration1 = 0;
            try
            {
                if (!string.IsNullOrEmpty(budget))
                { budget1 = (int)GetBudgetFromDescription(budget); }
                if (!string.IsNullOrEmpty(duration))
                { duration1 = (int)GetDurationFromDescription(duration); }
            }
            catch { }
            ListingViewModel pkgmodel = new ListingViewModel();
            List<PackageListViewModel> pkgList = new List<PackageListViewModel>();
            pkgmodel.PackageId = reqid ?? (pkgid ?? 0);


            //Request
            List<PackageListViewModel> FilterResultSession = new List<PackageListViewModel>();
            var SearchFilterResult = new List<PackageListViewModel>();
            if (!string.IsNullOrEmpty(searchType))
            {
                if (searchType == "filter")
                {
                    var PackageList = HttpContext.Session.Get<List<PackageListViewModel>>("PackageListResultSession");
                    FilterResultSession = PackageList;
                    if (FilterResultSession != null)
                    {
                        pkgmodel.PackagesList = FilterResultSession.OrderBy(o => o.PackagePrice).ToList();
                    }
                }
            }
            else
            {
                // var packages = _pkgservice.GetPackagesList();                
                //packages = packages.Where(x => x.IsPublish == true).ToList();
                var packages = _pkgservice.GetPackagesListByFutureDate();
                if (packages != null)
                {
                    if (pkgmodel.PackageId > 0)
                    {
                        packages = packages.Where(x => x.Id == pkgmodel.PackageId).ToList();
                    }
                    if (budget1 > 0)
                    {
                        packages = PkgBudgetFilter(budget1, packages);
                    }
                    if (duration1 > 0)
                    {
                        packages = PkgDurationFilter(duration1, packages).ToList();
                    }
                    if (!string.IsNullOrEmpty(regionId))
                    {
                        var regName = regionId.First().ToString().ToUpper() + String.Join("", regionId.Skip(1));
                        packages = packages.Where(x => x.Region.Description.Contains(regionId)).ToList();
                        //pkgmodel.RegionName = (packages != null ? string.Format("{0} {1}", packages.FirstOrDefault().Region.Name, "Europe") : "");
                        pkgmodel.RegionName = (packages != null ? (packages.Count > 0 ? string.Format("{0} {1}", packages.FirstOrDefault().Region.Name, "Europe") : "" + regName + " Europe") : "" + regName + " Europe");
                        pkgmodel.BestPlace = $"<h1> Best Holiday packages in {pkgmodel.RegionName}</h1>";

                    }
                    if (!string.IsNullOrEmpty(category))
                    {
                        var themes = _themeService.GetAllThemes(true, false);
                        var matchingThemes = themes.Where(theme => theme.ShortName.Contains(category)).ToList();

                        if (matchingThemes.Any())
                        {
                            var themeIds = matchingThemes.Select(theme => theme.Id).ToList();
                            packages = packages.Where(pkg => themeIds.Contains((int)pkg.ThemeId)).ToList();
                            pkgmodel.ThemeName = packages != null ? string.Join(", ", matchingThemes.Select(theme => theme.Name)) : "";
                            pkgmodel.BestPlace = $"<h1> Best {pkgmodel.ThemeName} as per your preference </h1>";
                        }
                    }
                    ////if (!string.IsNullOrEmpty(destination))
                    ////{
                    ////   var countries = _countryService.GetAllCountries(true, false);
                    ////    var matchingCountries = countries.Where(c => c.Name.Contains(destination)).ToList();

                    ////    if (matchingCountries.Any())
                    ////    {
                    ////        var countriesIds = matchingCountries.Select(c => c.Id).ToList();
                    ////        packages = packages.Where(pkg => countriesIds.Contains((int)pkg.CountryId)).ToList();
                    ////        pkgmodel.CountryName = packages != null ? string.Join(", ", matchingCountries.Select(c => c.Name)) : "";
                    ////    }
                    ////}

                    if (!string.IsNullOrEmpty(destination))
                    {
                        var countries = _countryService.GetCountrybyShortName(destination);
                        if (countries != null)
                        {
                            packages = packages.Where(pkg => (int)pkg.CountryId == countries.Id).ToList();
                            pkgmodel.CountryName = countries.Name;
                        }
                    }
                    if (IsCruseIncluded > 0)
                    {
                        packages = packages.Where(x => x.IsCruseIncluded == true).ToList();
                    }
                    pkgmodel.PackagesList = BindPackageListingData(packages, pkgList);
                    if (pkgmodel.PackagesList.Count() > 0)
                    {
                        //Country ADD
                        List<SelectListItem> CountryList = new List<SelectListItem>();
                        CountryList = pkgmodel.PackagesList.Select(x => new SelectListItem
                        {
                            Value = x.LocationId.ToString(),
                            Text = x.LocationAddress
                        }).ToList();

                        pkgmodel.CountryList = CountryList.OrderBy(x => x.Text).GroupBy(x => x.Text).Select(y => y.FirstOrDefault()).ToList();

                        HttpContext.Session.Set<List<PackageListViewModel>>("PackageListResultSession", pkgmodel.PackagesList);

                        decimal MinValue = 0, MaxValue = 0;
                        if (!string.IsNullOrEmpty(priceRange))
                        {
                            var price_Range = priceRange.Split(',');
                            MinValue = Convert.ToDecimal(price_Range[0]);
                            MaxValue = Convert.ToDecimal(price_Range[1]);
                            pkgmodel.PackagesList = pkgmodel.PackagesList.Where(S => (S.PackagePrice >= MinValue && S.PackagePrice <= MaxValue)).ToList();

                            ViewBag.v_PriceRange = "[" + priceRange + "]";
                        }

                        //var _highestPrice = pkgmodel.PackagesList.OrderByDescending(x => x.PackagePrice).FirstOrDefault();

                        //pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;

                        //ViewBag.v_PriceRange = _highestPrice != null ? "[0,"+ _highestPrice.PackagePrice + "]" : "[0,9000]";

                        FilterResultSession = pkgmodel.PackagesList.OrderBy(o => o.PackagePrice).ToList();
                    }
                    else
                    {
                        HttpContext.Session.Set<List<PackageListViewModel>>("PackageListResultSession", pkgmodel.PackagesList);
                    }

                    pkgmodel.totalItem = pkgmodel.PackagesList.Count;
                    pkgmodel.CurrentPageIndex = curentPage;

                    if (pageSize != -1)
                    {
                        pkgmodel.PackagesList = pkgmodel.PackagesList.OrderBy(o => o.PackagePrice).Skip((curentPage - 1) * pageSize).Take(skipSize).ToList();
                    }

                }
            }

            //Session NUlldata
            if (FilterResultSession == null)
            {
                //var packages = _pkgservice.GetPackagesList();
                //packages = packages.Where(x => x.IsPublish == true).ToList();
                var packages = _pkgservice.GetPackagesListByFutureDate();
                if (packages != null)
                {
                    if (pkgmodel.PackageId > 0)
                    {
                        packages = packages.Where(x => x.Id == pkgmodel.PackageId).ToList();
                    }
                    if (budget1 > 0)
                    {
                        packages = PkgBudgetFilter(budget1, packages);
                    }
                    if (duration1 > 0)
                    {
                        packages = PkgDurationFilter(duration1, packages).ToList();
                    }
                    if (!string.IsNullOrEmpty(regionId))
                    {
                        packages = packages.Where(x => x.Region.Description.Contains(regionId)).ToList();
                        pkgmodel.RegionName = (packages != null ? string.Format("{0} {1}", packages.FirstOrDefault().Region.Name, "Europe") : "");
                        pkgmodel.BestPlace = $"<h1> Best Holiday packages in {pkgmodel.RegionName}</h1>";

                    }
                    if (!string.IsNullOrEmpty(category))
                    {
                        var themes = _themeService.GetAllThemes(true, false);
                        var matchingThemes = themes.Where(theme => theme.ShortName.Contains(category)).ToList();

                        if (matchingThemes.Any())
                        {
                            var themeIds = matchingThemes.Select(theme => theme.Id).ToList();
                            packages = packages.Where(pkg => themeIds.Contains((int)pkg.ThemeId)).ToList();
                            pkgmodel.ThemeName = packages != null ? string.Join(", ", matchingThemes.Select(theme => theme.Name)) : "";
                            pkgmodel.BestPlace = $"<h1> Best {pkgmodel.ThemeName} as per your preference </h1>";
                        }
                    }
                    ////if (!string.IsNullOrEmpty(destination))
                    ////{
                    ////   var countries = _countryService.GetAllCountries(true, false);
                    ////    var matchingCountries = countries.Where(c => c.Name.Contains(destination)).ToList();

                    ////    if (matchingCountries.Any())
                    ////    {
                    ////        var countriesIds = matchingCountries.Select(c => c.Id).ToList();
                    ////        packages = packages.Where(pkg => countriesIds.Contains((int)pkg.CountryId)).ToList();
                    ////        pkgmodel.CountryName = packages != null ? string.Join(", ", matchingCountries.Select(c => c.Name)) : "";
                    ////    }
                    ////}

                    if (!string.IsNullOrEmpty(destination))
                    {
                        var countries = _countryService.GetCountrybyShortName(destination);
                        if (countries != null)
                        {
                            packages = packages.Where(pkg => (int)pkg.CountryId == countries.Id).ToList();
                            pkgmodel.CountryName = countries.Name;
                        }
                    }
                    if (IsCruseIncluded > 0)
                    {
                        packages = packages.Where(x => x.IsCruseIncluded == true).ToList();
                    }
                    pkgmodel.PackagesList = BindPackageListingData(packages, pkgList);
                    if (pkgmodel.PackagesList.Count() > 0)
                    {
                        //Country ADD
                        List<SelectListItem> CountryList = new List<SelectListItem>();
                        CountryList = pkgmodel.PackagesList.Select(x => new SelectListItem
                        {
                            Value = x.LocationId.ToString(),
                            Text = x.LocationAddress
                        }).ToList();

                        pkgmodel.CountryList = CountryList.OrderBy(x => x.Text).GroupBy(x => x.Text).Select(y => y.FirstOrDefault()).ToList();

                        HttpContext.Session.Set<List<PackageListViewModel>>("PackageListResultSession", pkgmodel.PackagesList);

                        decimal MinValue = 0, MaxValue = 0;
                        if (!string.IsNullOrEmpty(priceRange))
                        {
                            var price_Range = priceRange.Split(',');
                            MinValue = Convert.ToDecimal(price_Range[0]);
                            MaxValue = Convert.ToDecimal(price_Range[1]);
                            pkgmodel.PackagesList = pkgmodel.PackagesList.Where(S => (S.PackagePrice >= MinValue && S.PackagePrice <= MaxValue)).ToList();

                            ViewBag.v_PriceRange = "[" + priceRange + "]";
                        }

                        //var _highestPrice = pkgmodel.PackagesList.OrderByDescending(x => x.PackagePrice).FirstOrDefault();

                        //pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;

                        //ViewBag.v_PriceRange = _highestPrice != null ? "[0,"+ _highestPrice.PackagePrice + "]" : "[0,9000]";

                        FilterResultSession = pkgmodel.PackagesList.OrderBy(o => o.PackagePrice).ToList();
                    }
                    else
                    {
                        HttpContext.Session.Set<List<PackageListViewModel>>("PackageListResultSession", pkgmodel.PackagesList);
                    }
                    pkgmodel.totalItem = pkgmodel.PackagesList.Count;
                    pkgmodel.CurrentPageIndex = curentPage;

                    if (pageSize != -1)
                    {
                        pkgmodel.PackagesList = pkgmodel.PackagesList.OrderBy(o => o.PackagePrice).Skip((curentPage - 1) * pageSize).Take(skipSize).ToList();
                    }

                }
            }
            var _highestPrice = new PackageListViewModel();
            if (FilterResultSession != null && FilterResultSession.Count > 0)
            {
                _highestPrice = FilterResultSession.OrderByDescending(x => x.PackagePrice).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(budget))
            {
                var _budgets = budget.Replace("$", "").Replace(" ", "");
                if (_budgets == "9000")
                {
                    //pkgmodel.LowestPrice = 9000;
                    //pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                    //ViewBag.v_PriceRange = _highestPrice != null ? "[9000," + _highestPrice.PackagePrice + "]" : "[0,9000]";
                    //pkgmodel.MinPrice = 9000;
                    //pkgmodel.MaxPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;

                    int lowestval = _highestPrice != null ? _highestPrice.PackagePrice > 0 ? 9000 : 0 : 0;
                    if (lowestval > 0)
                    {
                        pkgmodel.LowestPrice = 9000;
                        pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                        ViewBag.v_PriceRange = _highestPrice != null ? "[9000," + _highestPrice.PackagePrice + "]" : "[0,9000]";
                        pkgmodel.MinPrice = 9000;
                        pkgmodel.MaxPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                        pkgmodel.MaxPriceFront = _highestPrice != null ? _highestPrice.PackagePrice.ToString("0.##") : "9000";
                    }
                    else
                    {
                        pkgmodel.LowestPrice = 0;
                        pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                        ViewBag.v_PriceRange = _highestPrice != null ? "[0," + _highestPrice.PackagePrice + "]" : "[0,9000]";
                        pkgmodel.MinPrice = 0;
                        pkgmodel.MaxPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                        pkgmodel.MaxPriceFront = _highestPrice != null ? _highestPrice.PackagePrice.ToString("0.##") : "9000";
                    }
                }
                else if (_budgets == "1000")
                {
                    pkgmodel.LowestPrice = 0;
                    pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 1000;
                    ViewBag.v_PriceRange = _highestPrice != null ? "[0," + _highestPrice.PackagePrice + "]" : "[0,1000]";
                    pkgmodel.MinPrice = 0;
                    pkgmodel.MaxPrice = _highestPrice != null ? _highestPrice.PackagePrice : 1000;
                    pkgmodel.MaxPriceFront = _highestPrice != null ? _highestPrice.PackagePrice.ToString("0.##") : "1000";
                }
                else
                {
                    var _budgetsplit = _budgets.Split("-");
                    pkgmodel.LowestPrice = _budgetsplit[0] != null ? Convert.ToDecimal(_budgetsplit[0]) : 0;
                    pkgmodel.HighestPrice = _budgetsplit[1] != null ? Convert.ToDecimal(_budgetsplit[1]) : _highestPrice != null ? _highestPrice.PackagePrice : 9000;

                    ViewBag.v_PriceRange = _highestPrice != null ? "[" + pkgmodel.LowestPrice + "," + pkgmodel.HighestPrice + "]" : "[0,9000]";
                    pkgmodel.MinPrice = pkgmodel.LowestPrice;
                    pkgmodel.MaxPrice = pkgmodel.HighestPrice;
                    pkgmodel.MaxPriceFront = pkgmodel.HighestPrice > 0 ? pkgmodel.HighestPrice.ToString("0.##") : "0";
                }

            }
            else
            {
                pkgmodel.LowestPrice = 0;
                pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                ViewBag.v_PriceRange = _highestPrice != null ? "[0," + _highestPrice.PackagePrice + "]" : "[0,9000]";
                pkgmodel.MinPrice = 0;
                pkgmodel.MaxPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;
                pkgmodel.MaxPriceFront = _highestPrice != null ? _highestPrice.PackagePrice.ToString("0.##") : "9000";
            }
            if (FilterResultSession != null && FilterResultSession.Count > 0)
            {
                //Filter
                if (!string.IsNullOrEmpty(searchType))
                {
                    if (searchType == "filter")
                    {
                        //Duration
                        bool IsDay1 = false, IsDay2 = false, IsDay3 = false, IsDay4 = false, IsDay5 = false;
                        if (!string.IsNullOrEmpty(durationVal))
                        {
                            var duratList = durationVal.Split(',');
                            for (int i = 0; i < duratList.Count(); i++)
                            {
                                switch (duratList[i])
                                {
                                    case "1":
                                        IsDay1 = true;
                                        break;
                                    case "2":
                                        IsDay2 = true;
                                        break;
                                    case "3":
                                        IsDay3 = true;
                                        break;
                                    case "4":
                                        IsDay4 = true;
                                        break;
                                    case "5":
                                        IsDay5 = true;
                                        break;
                                }
                            }


                            //SearchFilterResult = FilterResultSession
                            //    .Where(S => (IsDay1 == true && S.PackageNoOfDays <= 3)
                            //    || (IsDay2 == true && (S.PackageNoOfDays > 3 && S.PackageNoOfDays < 7))
                            //    || (IsDay3 == true && (S.PackageNoOfDays > 6 && S.PackageNoOfDays < 10))
                            //    || (IsDay4 == true && (S.PackageNoOfDays > 9 && S.PackageNoOfDays < 13))
                            //    || (IsDay5 == true && (S.PackageNoOfDays > 13))
                            //    ).ToList();


                        }

                        //Inclusion
                        bool IsMeal = false, IsCruse = false, IsTransport = false, IsHotel = false, IsTransfer = false;
                        if (!string.IsNullOrEmpty(inclusionVal))
                        {
                            var inclusList = inclusionVal.Split(',');
                            for (int i = 0; i < inclusList.Count(); i++)
                            {
                                switch (inclusList[i])
                                {
                                    case "1":
                                        IsMeal = true;
                                        break;
                                    case "2":
                                        IsCruse = true;
                                        break;
                                    case "3":
                                        IsTransport = true;
                                        break;
                                    case "4":
                                        IsHotel = true;
                                        break;
                                    case "5":
                                        IsTransfer = true;
                                        break;
                                }
                            }

                            //if (SearchFilterResult.Count() > 0)
                            //{
                            //    SearchFilterResult = SearchFilterResult
                            //    .Where(S => (IsMeal == true && S.IsMealIncluded == IsMeal)
                            //    || (IsCruse == true && S.IsCruseIncluded == IsCruse)
                            //    || (IsTransport == true && S.IsTransportIncluded == IsTransport)
                            //    || (IsHotel == true && S.IsHotelIncluded == IsHotel)
                            //    || (IsTransfer == true && S.IsTransferIncluded == IsTransfer)
                            //    ).ToList();
                            //}
                            //else
                            //{
                            //    SearchFilterResult = FilterResultSession
                            //    .Where(S => (IsMeal == true && S.IsMealIncluded == IsMeal)
                            //    || (IsCruse == true && S.IsCruseIncluded == IsCruse)
                            //    || (IsTransport == true && S.IsTransportIncluded == IsTransport)
                            //    || (IsHotel == true && S.IsHotelIncluded == IsHotel)
                            //    || (IsTransfer == true && S.IsTransferIncluded == IsTransfer)
                            //    ).ToList();
                            //}
                        }

                        //Rating
                        bool IsRating1 = false, IsRating2 = false, IsRating3 = false, IsRating4 = false, IsRating5 = false;
                        if (!string.IsNullOrEmpty(ratingVal))
                        {
                            var ratnList = ratingVal.Split(',');
                            if (ratnList.Count() == 5)
                            {
                                ratingVal = "";
                            }
                            else
                            {
                                for (int i = 0; i < ratnList.Count(); i++)
                                {
                                    switch (ratnList[i])
                                    {
                                        case "1":
                                            IsRating1 = true;
                                            break;
                                        case "2":
                                            IsRating2 = true;
                                            break;
                                        case "3":
                                            IsRating3 = true;
                                            break;
                                        case "4":
                                            IsRating4 = true;
                                            break;
                                        case "5":
                                            IsRating5 = true;
                                            break;
                                    }
                                }
                            }


                        }

                        //Country
                        //string[] countryList=new string[""]
                        string[] countryList = { "" };
                        if (!string.IsNullOrEmpty(countryVal))
                        {
                            countryList = countryVal.Split(',');

                            //Filtter
                            //SearchFilterResult = FilterResultSession
                            //    .Where(S =>
                            //    (countryList.Contains(S.LocationId.ToString()))
                            //    &&
                            //    ((IsDay1 == true && S.PackageNoOfDays <= 3)
                            //    || (IsDay2 == true && (S.PackageNoOfDays > 3 && S.PackageNoOfDays < 7))
                            //    || (IsDay3 == true && (S.PackageNoOfDays > 6 && S.PackageNoOfDays < 10))
                            //    || (IsDay4 == true && (S.PackageNoOfDays > 9 && S.PackageNoOfDays < 13))
                            //    || (IsDay5 == true && (S.PackageNoOfDays > 13)))

                            //    && ((IsMeal == true && S.IsMealIncluded == IsMeal)
                            //    || (IsCruse == true && S.IsCruseIncluded == IsCruse)
                            //    || (IsTransport == true && S.IsTransportIncluded == IsTransport)
                            //    || (IsHotel == true && S.IsHotelIncluded == IsHotel)
                            //    || (IsTransfer == true && S.IsTransferIncluded == IsTransfer))

                            //    && ((IsRating2 == true && S.Rating == 2)
                            //    || (IsRating3 == true && S.Rating == 3)
                            //    || (IsRating4 == true && S.Rating == 4)
                            //    || (IsRating5 == true && S.Rating == 5))

                            //    ).ToList();



                            //if (SearchFilterResult.Count() > 0)
                            //{
                            //    SearchFilterResult = SearchFilterResult
                            //    .Where(S => countryList.Contains(S.LocationId.ToString())).ToList();
                            //}
                            //else
                            //{
                            //    SearchFilterResult = FilterResultSession
                            //     .Where(S => countryList.Contains(S.LocationId.ToString())).ToList();
                            //}
                        }

                        //Direction
                        string[] directionList = { "" };
                        if (!string.IsNullOrEmpty(directionVal))
                        {
                            directionList = directionVal.Split(',');
                        }

                        SearchFilterResult = FilterResultSession
                                 .Where(S => (!string.IsNullOrEmpty(durationVal) ? ((IsDay1 == true && S.PackageNoOfDays <= 3)
                                 || (IsDay2 == true && (S.PackageNoOfDays > 3 && S.PackageNoOfDays < 7))
                                 || (IsDay3 == true && (S.PackageNoOfDays > 6 && S.PackageNoOfDays < 10))
                                 || (IsDay4 == true && (S.PackageNoOfDays > 9 && S.PackageNoOfDays < 13))
                                 || (IsDay5 == true && (S.PackageNoOfDays > 13))) : true)

                                 //&& (!string.IsNullOrEmpty(inclusionVal) ?
                                 // ((IsMeal == true && S.IsMealIncluded == IsMeal)
                                 //|| (IsCruse == true && S.IsCruseIncluded == IsCruse)
                                 //|| (IsTransport == true && S.IsTransportIncluded == IsTransport)
                                 //|| (IsHotel == true && S.IsHotelIncluded == IsHotel)
                                 //|| (IsTransfer == true && S.IsTransferIncluded == IsTransfer)) : true)

                                  && (!string.IsNullOrEmpty(inclusionVal) ?
                                  ((IsMeal == true ? S.IsMealIncluded == IsMeal : true)
                                 && (IsCruse == true ? S.IsCruseIncluded == IsCruse : true)
                                 && (IsTransport == true ? S.IsTransportIncluded == IsTransport : true)
                                 && (IsHotel == true ? S.IsHotelIncluded == IsHotel : true)
                                 && (IsTransfer == true ? S.IsTransferIncluded == IsTransfer : true)) : true)

                                  && (!string.IsNullOrEmpty(ratingVal) ?
                                  ((IsRating1 == true && S.Rating == 1)
                                  || (IsRating2 == true && S.Rating == 2)
                                  || (IsRating3 == true && S.Rating == 3)
                                  || (IsRating4 == true && S.Rating == 4)
                                  || (IsRating5 == true && S.Rating == 5)) : true)
                                  && (!string.IsNullOrEmpty(countryVal) ? (countryList.Contains(S.LocationId.ToString())) : true)
                                  && (!string.IsNullOrEmpty(directionVal) ? (directionList.Contains(S.RegionId.ToString())) : true)
                                 ).ToList();


                        decimal MinValue = 0, MaxValue = 0;
                        if (!string.IsNullOrEmpty(priceRange))
                        {
                            var price_Range = priceRange.Split(',');
                            MinValue = Convert.ToDecimal(price_Range[0]);
                            MaxValue = Convert.ToDecimal(price_Range[1]);
                            SearchFilterResult = SearchFilterResult.Where(S => (S.PackagePrice >= MinValue && S.PackagePrice <= MaxValue)).ToList();
                            ViewBag.v_PriceRange = "[" + priceRange + "]";

                            pkgmodel.MinPrice = MinValue;
                            pkgmodel.MaxPrice = MaxValue > 0 ? MaxValue : 9000;
                            pkgmodel.MaxPriceFront = MaxValue > 0 ? MaxValue.ToString("0.##") : "9000";
                        }

                        //var _highestPrice = SearchFilterResult.OrderByDescending(x => x.PackagePrice).FirstOrDefault();

                        //pkgmodel.HighestPrice = _highestPrice != null ? _highestPrice.PackagePrice : 9000;

                        pkgmodel.PackagesList = SearchFilterResult.OrderBy(o => o.PackagePrice).ToList();
                        pkgmodel.totalItem = SearchFilterResult.Count;
                        pkgmodel.CurrentPageIndex = curentPage;

                        if (pageSize != -1)
                        {
                            pkgmodel.PackagesList = SearchFilterResult.OrderBy(o => o.PackagePrice).Skip((curentPage - 1) * pageSize).Take(skipSize).ToList();
                        }



                    }
                }
            }
            #region "Pager and Sorting"

            string _budget = string.IsNullOrEmpty(budget) ? "" : "?budget=" + budget;
            string _destination = string.IsNullOrEmpty(destination) ? "" : (_budget == "" ? "?" : "&") + "destination=" + destination;
            string _duration = string.IsNullOrEmpty(duration) ? "" : ((_budget == "" && _destination == "") ? "?" : "&") + "duration=" + duration;
            string _category = string.IsNullOrEmpty(category) ? "" : ((_budget == "" && _destination == "" && _duration == "") ? "?" : "&") + "category=" + category;
            string _reqid = reqid == null ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "") ? "?" : "&") + "reqid=" + reqid;
            string _regionId = string.IsNullOrEmpty(regionId) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "") ? "?" : "&") + "regionId=" + regionId;
            string _IsCruseIncluded = IsCruseIncluded == null ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "") ? "?" : "&") + "IsCruseIncluded=" + IsCruseIncluded;
            string _pkgid = pkgid == null ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "") ? "?" : "&") + "pkgid=" + pkgid;
            string _type = string.IsNullOrEmpty(searchType) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "") ? "?" : "&") + "searchType=" + searchType;

            string _durationVal = string.IsNullOrEmpty(durationVal) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "") ? "?" : "&") + "durationVal=" + durationVal;
            string _inclusionVal = string.IsNullOrEmpty(inclusionVal) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "" && _durationVal == "") ? "?" : "&") + "inclusionVal=" + inclusionVal;
            string _ratingVal = string.IsNullOrEmpty(ratingVal) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "" && _durationVal == "" && _inclusionVal == "") ? "?" : "&") + "ratingVal=" + ratingVal;
            string _PriceRange = string.IsNullOrEmpty(priceRange) ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "" && _durationVal == "" && _inclusionVal == "" && _ratingVal == "") ? "?" : "&") + "priceRange=" + priceRange;


            string _curentPage = curentPage == 0 ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "" && _durationVal == "" && _inclusionVal == "" && _ratingVal == "") ? "?" : "&") + "curentPage=" + curentPage;
            string _pageSize = pageSize == 0 ? "" : ((_budget == "" && _destination == "" && _duration == "" && _category == "" && _reqid == "" && _regionId == "" && _IsCruseIncluded == "" && _pkgid == "" && _type == "" && _durationVal == "" && _inclusionVal == "" && _ratingVal == "" && _curentPage == "") ? "?" : "&") + "pageSize=" + pageSize;

            string filters = _budget + _destination + _duration + _category + _reqid + _regionId + _IsCruseIncluded + _pkgid + _type + _durationVal + _inclusionVal + _ratingVal + _PriceRange;
            string URL = (filters.Length > 0 ? filters + "" : "?");

            ViewBag.URLs = URL;

            #endregion

            // pkgmodel.BudgetList = GetBudgetLists(budget1 ?? 3);
            pkgmodel.DurationList = GetDurationLists(duration1 ?? 3);
            pkgmodel.HotelRatings = GetHotelRatingLists(3);
            pkgmodel.PackageInclusionslst = GetallPackageInclusions(2);
            pkgmodel.PackageDirectionlst = _regionService.GetAllRegions(true, false).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();
            return pkgmodel;
        }


        [HttpGet]
        public IActionResult Index(string? budget, string? destination, string? duration, string? category, int? reqid, string? regionId, int? IsCruseIncluded, int? pkgid, int curentPage = 1, int pageSize = 6, string? searchType = "", string? durationVal = "", string inclusionVal = "", string ratingVal = "", string priceRange = "", string countryVal = "", string directionVal = "")
        {
            ViewBag.v_budget = budget;
            ViewBag.v_destination = destination;
            ViewBag.v_duration = duration;
            ViewBag.v_category = category;
            ViewBag.v_reqid = reqid;
            ViewBag.v_regionId = regionId;
            ViewBag.v_isCruseIncluded = IsCruseIncluded;
            ViewBag.v_pkgid = pkgid;
            ViewBag.v_curentPage = curentPage;
            ViewBag.v_pageSize = pageSize;
            ViewBag.v_countryVal = countryVal;


            ViewBag.v_durationVal = durationVal;
            ViewBag.v_hdinclusionVal = inclusionVal;
            ViewBag.v_hdratingVal = ratingVal;
            ViewBag.v_PriceRange = "[0,9000]";
            ViewBag.v_hdDirectionVal = directionVal;

            if (!string.IsNullOrEmpty(budget))
            {
                ViewBag.v_budgetTitle = budget == "$1000" ? "Less than " + budget : budget;
            }

            if (!string.IsNullOrEmpty(destination))
            {
                var countries = _countryService.GetCountrybyShortName(destination);
                ViewBag.v_destinationName = countries.Name.ToString();
            }

            int? budget1 = 0;
            int? duration1 = 0;
            try
            {
                if (!string.IsNullOrEmpty(budget))
                { budget1 = (int)GetBudgetFromDescription(budget); }
                if (!string.IsNullOrEmpty(duration))
                { duration1 = (int)GetDurationFromDescription(duration); }
            }
            catch { }

            if (!string.IsNullOrEmpty(durationVal))
            {
                ViewBag.v_durationVal = durationVal;
            }
            else
            {
                if (!string.IsNullOrEmpty(duration))
                {
                    ViewBag.v_durationVal = duration1.ToString();
                }
            }
            if (!string.IsNullOrEmpty(inclusionVal))
            {
                ViewBag.v_hdinclusionVal = inclusionVal;
            }
            else
            {
                if (IsCruseIncluded == 1)
                {
                    ViewBag.v_hdinclusionVal = "2";
                }
            }
            if (!string.IsNullOrEmpty(countryVal))
            {
                ViewBag.v_countryVal = countryVal;
            }
            else
            {
                if (!string.IsNullOrEmpty(destination))
                {
                    var countries = _countryService.GetCountrybyShortName(destination);
                    ViewBag.v_countryVal = countries.Id.ToString();
                }
            }
            //if (!string.IsNullOrEmpty(directionVal))
            //{
            //    ViewBag.v_hdDirectionVal = directionVal;
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(regionId))
            //    {
            //        ViewBag.v_hdDirectionVal = regionId.ToString();
            //    }
            //}
            //int skipSize = curentPage * pageSize;
            int skipSize = 6;
            if (string.IsNullOrEmpty(destination) && budget1 == 0 && duration1 == 0 && string.IsNullOrEmpty(category) && string.IsNullOrEmpty(regionId) && pkgid == 0)
            {
                ShowErrorMessage("Error", "Please select some package/category/regionId or enter any of the (destination, duration, budget) ", false);
                return RedirectToAction("Index", "Home");
            }

            ListingViewModel model = new ListingViewModel();
            model = GetProductSearchResult(budget, destination, duration, category, reqid, regionId, IsCruseIncluded, pkgid, curentPage, pageSize, searchType, durationVal, inclusionVal, ratingVal, priceRange, countryVal, directionVal);

            return View(model);
        }
        private List<Package> PkgDurationFilter(int? duration, List<Package> packages)
        {
            switch (duration)
            {
                case (int)PkgListingByDuration.Days1to3:
                    packages = packages.Where(x => x.PackageNoOfDays >= 1 && x.PackageNoOfDays <= 3).ToList();
                    break;
                case (int)PkgListingByDuration.Days4to6:
                    packages = packages.Where(x => x.PackageNoOfDays >= 4 && x.PackageNoOfDays <= 6).ToList();/////
                    break;
                case (int)PkgListingByDuration.Days7to9:
                    packages = packages.Where(x => x.PackageNoOfDays >= 7 && x.PackageNoOfDays <= 9).ToList();
                    break;
                case (int)PkgListingByDuration.Days10to12:
                    packages = packages.Where(x => x.PackageNoOfDays >= 10 && x.PackageNoOfDays <= 12).ToList();
                    break;
                case (int)PkgListingByDuration.GreaterThan13days:
                    packages = packages.Where(x => x.PackageNoOfDays > 13).ToList();
                    break;
                default:
                    packages = packages.Where(x => x.PackageNoOfDays >= 13 && x.PackageNoOfDays <= 15).ToList();
                    break;
            }
            return packages;

        }
        private List<Package> PkgBudgetFilter(int? budget, List<Package> packages)
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


        private List<PackageListViewModel> BindPackageListingData(List<Package> packages, List<PackageListViewModel> pkgList)
        {
            foreach (var item in packages)
            {

                PackageListViewModel pkgObj = new PackageListViewModel();
                pkgObj.PackageId = item.Id;
                pkgObj.LocationAddress = item.Country?.Name ?? "-";
                pkgObj.LocationId = item.Country?.Id;
                pkgObj.PkgDtlId = item.Id;
                pkgObj.IsCruseIncluded = item.IsCruseIncluded;
                pkgObj.IsTransferIncluded = item.IsTransferIncluded;
                pkgObj.IsTransportIncluded = item.IsTransportIncluded;
                pkgObj.IsHotelIncluded = item.IsHotelIncluded;
                pkgObj.IsMealIncluded = item.IsMealIncluded;
                pkgObj.PackagePrice = item.PackagePrice;
                pkgObj.PackageUrl = item.PackageUrl;
                pkgObj.PackagePriceFront = item.PackagePrice.ToString("0.##");
                pkgObj.PackageName = item.Name;
                pkgObj.PkgDesc = item.Description?.Length <= 83 ? item.Description : item.Description?.Substring(0, 83) + "..."; ;
                pkgObj.PackageNoOf_DaysNight = item.PackageNoOfDays.ToString() + "Days & " + item.PackageNoOfNights.ToString() + "Nights";
                pkgObj.PackageNoOfDays = item.PackageNoOfDays;
                pkgObj.RegionId = item.RegionId;
                var imagesObj = item.PackageImages?.FirstOrDefault();
                if (imagesObj != null)
                {
                    pkgObj.FileOriginalName = imagesObj.OriginalImageName ?? "";
                    pkgObj.FileExtension = imagesObj.ImageExtension;
                    pkgObj.PkgImgId = imagesObj.Id;
                    //pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin((UploadSection)imagesObj.ImageSection, imagesObj.ImageName, imagesObj.ImageExtension, imagesObj.OriginalImageName);
                    string RetImageName = "";
                    pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);
                    imagesObj.ImageName = RetImageName;
                }

                var pkgDtls = item.PackageDetails?.FirstOrDefault();
                if (pkgDtls != null)
                {
                    var hoteldtls = pkgDtls.PackageHotels.FirstOrDefault();
                    if (hoteldtls != null)
                    {
                        pkgObj.Rating = hoteldtls.Hotel?.Rating ?? 0;
                    }
                }
                pkgList.Add(pkgObj);
            }
            return pkgList;
        }


        private List<EnumsList> GetallPackageInclusions(int? inclusionId)
        {
            List<EnumsList> lst = Enum.GetValues(typeof(PkgInclusions))
                               .Cast<PkgInclusions>()
                               .Select(g => new EnumsList
                               {
                                   Id = Convert.ToInt16(g),
                                   Name = g.GetDescription(),
                                   //active = Convert.ToInt16(g) == noofStar ? "active" : ""
                               })
                               .ToList();
            return lst;

        }
        private List<EnumsList> GetHotelRatingLists(int? noofStar)
        {
            List<EnumsList> lst = Enum.GetValues(typeof(HotelRating))
                               .Cast<HotelRating>()
                               .Select(g => new EnumsList
                               {
                                   Id = Convert.ToInt16(g),
                                   Name = g.GetDescription(),
                                   //Active = Convert.ToInt16(g) == noofStar ? "active" : ""
                               })
                               .ToList();
            return lst;

        }


        private List<EnumsList> GetDurationLists(int? activeduration)
        {
            List<EnumsList> lst = Enum.GetValues(typeof(PkgListingByDuration))
                               .Cast<PkgListingByDuration>()
                               .Select(g => new EnumsList
                               {
                                   Id = Convert.ToInt16(g),
                                   Name = g.GetDescription(),
                                   Active = Convert.ToInt16(g) == activeduration ? "active" : ""
                               })
                               .ToList();
            return lst;

        }


        public PkgListingByDuration GetDurationFromDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                var enumType = typeof(PkgListingByDuration);
                var values = Enum.GetValues(enumType);

                foreach (var value in values)
                {
                    FieldInfo field = enumType.GetField(value.ToString());
                    DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

                    if (attribute != null && attribute.Description.Contains(description))
                    {
                        return (PkgListingByDuration)value;
                    }
                }
            }
            return PkgListingByDuration.GreaterThan13days;
        }


        public PricedPkgInBudget GetBudgetFromDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                var enumType = typeof(PricedPkgInBudget);
                var values = Enum.GetValues(enumType);

                foreach (var value in values)
                {
                    FieldInfo field = enumType.GetField(value.ToString());
                    DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

                    if (attribute != null && attribute.Description.Replace(" ", "").Contains(description.Replace(" ", "")))
                    {
                        return (PricedPkgInBudget)value;
                    }
                }
            }
            return PricedPkgInBudget.GreaterThan9K;
        }

        ////private List<EnumsList> GetDistinctDurationsByPackage(List<Package> packages)
        ////{
        ////    var distinctDurations = packages
        ////        .Select(pkg => pkg.Duration)
        ////        .Distinct()
        ////        .OrderBy(duration => duration) 
        ////        .ToList();

        ////    var durationList = distinctDurations
        ////        .Select((duration, index) => new EnumsList
        ////        {
        ////            Id = index + 1, 
        ////            Name = duration.ToString(), 
        ////            Active = duration == (PkgListingByDuration)(duration ?? 3) ? "active" : ""
        ////        })
        ////        .ToList();

        ////    return durationList;
        ////}
    }
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                return default;

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
