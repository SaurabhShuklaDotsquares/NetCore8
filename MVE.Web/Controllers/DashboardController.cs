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
using static TCP.Web.ViewModels.CommonFileViewModel;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using TCP.Core;
using System.Globalization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TCP.Core.Models.Others;
using System.Diagnostics.Metrics;
using TCP.Core.Code.Notification;
using TCP.Web.CommonClass;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using AspNetCore;

namespace TCP.Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ISettingService _settingService;
        private readonly IUserService _userService;
        private readonly IPackageService _packageService;
        private readonly ICountryService _countryService;
        private readonly ICheckoutService _checkoutService;
        private readonly IContactUsService _contactService;
        private readonly IRatingReviewService _ratingReviewService;
        private readonly INotificationService _notificationService;
        public DashboardController(INotificationService notificationService, ILogger<DashboardController> logger, ISettingService settingService, IUserService userService
            , IPackageService packageService, ICountryService countryService, ICheckoutService checkoutService, IContactUsService contactService, IRatingReviewService ratingReviewService)
        {
            _logger = logger;
            _settingService = settingService;
            _userService = userService;
            _packageService = packageService;
            _countryService = countryService;
            _checkoutService = checkoutService;
            _contactService = contactService;
            _ratingReviewService = ratingReviewService;
            _notificationService = notificationService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            DashboardViewModel vmodel = new DashboardViewModel();
            List<PackageListViewModel> pkgList = new List<PackageListViewModel>();
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Login Please", false);
                return RedirectToAction("Index", "Home");
            }
            var bookingDtls = _checkoutService.GetBookingByUserId(CurrentFrontUser.Id);
            if (bookingDtls != null)
            {
                foreach (var item in bookingDtls)
                {
                    var packages = _packageService.GetPackagesDashboardList(item.PackageId).ToList();
                    if (packages.Any())
                    {
                        vmodel.TotalBookings = bookingDtls.Count;
                        vmodel.TotalSpentBookings = bookingDtls.Sum(x => Convert.ToInt32(x.TotalPrice));

                        vmodel.BookingPkgLst = BindPackageListData(packages, pkgList, item).OrderByDescending(x => x.BookingNo).Take(5).ToList();
                        vmodel.UpcomingBookingPkgLst = pkgList.Where(x => x.FromDatetime >= DateTime.Now.Date).OrderByDescending(x => x.BookingNo).ToList();
                    }

                }

            }



            return View(vmodel);
        }
        //private List<MyBookingDto> BindBookingPackageData(List<Booking> bookings)
        //{
        //    List<MyBookingDto> mybookingList = new List<MyBookingDto>();

        //    foreach (var item in bookings)
        //    {
        //        MyBookingDto myBookingDto = new MyBookingDto();
        //        myBookingDto.BookingNo = item.Id;
        //        if (item.Package != null)
        //        {
        //            myBookingDto.PackageId = item.Package.Id;
        //            myBookingDto.PackageName = item.Package.Name;
        //            myBookingDto.PackagePrice = item.Package.PackagePrice;
        //            myBookingDto.PackageStartDate = item.Package.FromDate.ToString("dd MMM, yyyy");
        //            myBookingDto.StartDate = item.Package.FromDate;
        //            myBookingDto.Description = item.Package.Description;

        //            myBookingDto.PkgDesc = item.Package.Description?.Length <= 203 ? item.Package.Description : TrimHtmlText(item.Package.Description, 200);
        //            myBookingDto.PackageDuration = item.Package.PackageNoOfDays.ToString() + " Days & " + item.Package.PackageNoOfNights.ToString() + " Nights";

        //            var getpackage = _packageService.GetActivePackageById(item.PackageId);
        //            var imagesObj = getpackage.PackageImages?.FirstOrDefault();
        //            if (imagesObj != null)
        //            {
        //                string RetImageName = "";
        //                myBookingDto.PackageImage = GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);

        //            }
        //        }
        //        mybookingList.Add(myBookingDto);
        //    }

        //    return mybookingList;
        //}
        private List<PackageListViewModel> BindPackageListData(List<Package> packages, List<PackageListViewModel> pkgList, Booking booking)
        {
            foreach (var item in packages)
            {

                PackageListViewModel pkgObj = new PackageListViewModel();
                pkgObj.PackageId = item.Id;
                pkgObj.PackageUrl = item.PackageUrl;
                pkgObj.LocationAddress = item.Country?.Name ?? "-";
                pkgObj.LocationId = item.Country?.Id;
                pkgObj.PkgDtlId = item.Id;
                pkgObj.IsCruseIncluded = item.IsCruseIncluded;
                pkgObj.PackagePrice = booking.TotalPrice; // Math.Floor(item.PackagePrice);
                pkgObj.PackagePriceFront = booking.TotalPrice.ToString("0.##"); // Math.Floor(item.PackagePrice);
                pkgObj.BookingNo = booking.Id;
                pkgObj.PackageName = item.Name;
                //pkgObj.FromDate = booking.CreationOn.ToString("dd MMM, yyyy");
                pkgObj.FromDate = item.FromDate.ToString("dd MMM, yyyy");
                pkgObj.FromDatetime = item.FromDate;
                pkgObj.PkgDesc = item.Description?.Length <= 203 ? item.Description : TrimHtmlText(item.Description, 200);
                pkgObj.PackageNoOf_DaysNight = item.PackageNoOfDays.ToString() + "Days & " + item.PackageNoOfNights.ToString() + "Nights";
                var imagesObj = item.PackageImages?.FirstOrDefault();
                if (imagesObj != null)
                {
                    pkgObj.FileOriginalName = imagesObj.OriginalImageName ?? "";
                    pkgObj.FileExtension = imagesObj.ImageExtension;
                    pkgObj.PkgImgId = imagesObj.Id;

                    string RetImageName = "";
                    pkgObj.FilePath = GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);
                    // imagesObj.ImageName = RetImageName;
                }
                else
                {
                    pkgObj.FilePath = SiteKeys.NoImagePath_Square;
                }
                pkgList.Add(pkgObj);
            }
            return pkgList;
        }


        //public IActionResult MyBooking(List<ThemeViewModel> model)
        //{
        //    var bookingDtls = _checkoutService.GetBookingByUserId(10);
        //    if (bookingDtls != null)
        //    {
        //        BindBookingPackageData(bookingDtls);
        //    }
        //    return PartialView("_MyBooking", model);
        //}
        public IActionResult MyBookingOld()
        {
            MyBookingViewModel myBookingViewModel = new MyBookingViewModel();
            var bookingDtls = _checkoutService.GetBookingByUserId(CurrentFrontUser.Id);
            List<MyBookingDto> bookings = new List<MyBookingDto>();
            if (bookingDtls != null)
            {
                bookings = BindBookingPackageData(bookingDtls);
            }
            myBookingViewModel.AllBooking = bookings;
            myBookingViewModel.UpcomingBooking = bookings.Where(x => x.StartDate.Date >= DateTime.Now.Date).ToList(); ;
            return View(myBookingViewModel);
        }

        #region MyProfile
        [HttpGet]
        public IActionResult MyProfile()
        {
            MyProfileViewModel myProfileView = new MyProfileViewModel();
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Login Please", false);
                return RedirectToAction("Index", "Home");
            }
            var userDtls = _userService.GetUserById(CurrentFrontUser.Id);
            if (userDtls != null)
            {
                myProfileView.PersonalObj = BindUserData(userDtls);
            }
            return PartialView("_MyProfile", myProfileView);
        }

        [HttpGet]
        public IActionResult PersonalDetail()
        {
            PersonalViewModel viewModel = new PersonalViewModel();
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Login Please", false);
                return RedirectToAction("Index", "Home");
            }
            var userDtls = _userService.GetUserById(CurrentFrontUser.Id);
            if (userDtls != null)
            {
                viewModel = BindUserData(userDtls);
            }
            return PartialView("_PersonalDetail", viewModel);
        }

        private PersonalViewModel BindUserData(User userDtls)
        {
            PersonalViewModel personalView = new PersonalViewModel();
            personalView.FullName = GetFullName(userDtls.FirstName ?? "", userDtls.LastName ?? "");
            personalView.FirstName = userDtls.FirstName;
            personalView.LastName = userDtls.LastName;
            personalView.UserId = userDtls.Id;
            personalView.EmailAddress = userDtls.Email ?? "NA";
            personalView.MobilePhone = userDtls.MobilePhone ?? "NA";
            personalView.Address = userDtls.Address ;
            personalView.DateofBirth = userDtls.DateOfBirth?.ToString(SiteKeys.DateFormatWithoutTime);
            return personalView;
        }


        [HttpGet]
        public IActionResult PersonalEdit()
        {
            PersonalViewModel model = new PersonalViewModel();
            var userDtls = _userService.GetUserById(CurrentFrontUser.Id);
            if (userDtls != null)
            {
                model = BindUserData(userDtls);
            }
            return PartialView("_PersonalEdit", model);
        }

        [HttpPost]
        public IActionResult PersonalEdit(PersonalViewModel model)
        {
            try
            {
                var userDtls = _userService.GetUserById(CurrentFrontUser.Id);
                if (userDtls != null)
                {
                    userDtls.FirstName = model.FirstName?.Trim();
                    userDtls.LastName = model.LastName?.Trim();
                    userDtls.Email = model.EmailAddress?.Trim();
                    userDtls.MobilePhone = model.MobilePhone?.Trim();
                    userDtls.Address = model.Address?.Trim();
                    userDtls.DateOfBirth = !string.IsNullOrEmpty(model.DateofBirth) ? DateTime.ParseExact(model.DateofBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.Now;
                    userDtls.ModifiedBy = CurrentFrontUser.Id;
                    userDtls.ModifiedOn = DateTime.UtcNow;
                    _userService.UpdateUser(userDtls);

                }
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Personal profile updated successfully", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some error occured", IsSuccess = false });
            }
        }


        private BillingViewModel BindUserBillingData(Billing billingDtls)
        {
            BillingViewModel billingView = new BillingViewModel();
            billingView.BillingId = billingDtls.Id;
            billingView.Address1 = billingDtls.Address1 ?? "NA";
            billingView.Address2 = billingDtls.Address2 ?? "NA";
            billingView.TownorCity = billingDtls.Town ?? "NA";
            billingView.StateId = billingDtls.StateId ?? 0;
            billingView.CountryId = billingDtls.Country ?? 0;
            billingView.CountryName = (billingDtls.Country ?? 0) != 0 ? _countryService.GetCountryMasterById(billingDtls.Country ?? 0).Name ?? "" : "NA";
            billingView.StateName = (billingDtls.StateId ?? 0) != 0 ? _countryService.GetStateMasterById(billingDtls.StateId ?? 0).Name ?? "" : "NA";
            billingView.ZipCode = billingDtls?.Zipcode ?? "NA";
            return billingView;
        }

        [HttpGet]
        public IActionResult BillingDetail()
        {
            BillingViewModel viewModel = new BillingViewModel();
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Login Please", false);
                return RedirectToAction("Index", "Home");
            }
            var billing = _userService.GetBillingByUserId(CurrentFrontUser.Id);
            if (billing != null)
            {
                viewModel = BindUserBillingData(billing);
            }
            return PartialView("_BillingDetail", viewModel);
        }
        [HttpGet]
        public IActionResult BillingEdit()
        {
            BillingViewModel viewModel = new BillingViewModel();
            var billing = _userService.GetBillingByUserId(CurrentFrontUser.Id);
            if (billing != null)
            {
                viewModel = BindUserBillingData(billing);
            }
            viewModel.CountryList = _countryService.GetAllCountries(true, false).Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}", Selected = c.Id == viewModel?.CountryId }).ToList();
            viewModel.CountryList.Insert(0, new SelectListItem() { Text = "-- Select --", Value = "0" });
            return PartialView("_BillingEdit", viewModel);
        }

        [HttpPost]
        public IActionResult BillingEdit(BillingViewModel model)
        {
            try
            {
                var userDtls = _userService.GetBillingById(model.BillingId);
                if (userDtls != null)
                {
                    userDtls.Address1 = model.Address1?.Trim();
                    userDtls.Address2 = model.Address2?.Trim();
                    userDtls.Town = model.TownorCity?.Trim();
                    userDtls.Country = model.CountryId;
                    userDtls.StateId = model.StateId;
                    if ((userDtls.Country ?? 0) == 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please select country", IsSuccess = false });
                    }
                    if ((userDtls.StateId ?? 0) == 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please select state", IsSuccess = false });
                    }
                    userDtls.Zipcode = model.ZipCode?.Trim();
                    userDtls.ModifiedBy = CurrentFrontUser.Id;
                    userDtls.ModifiedOn = DateTime.UtcNow;
                    _userService.UpdateBilling(userDtls);
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Billing profile updated successfully", IsSuccess = true });

                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "No billing details", IsSuccess = false });

                }

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some error occured", IsSuccess = false });
            }
        }
        #endregion

        #region [ Front User - Contact Us ]
        // contact us get
        [HttpGet]
        public IActionResult ContactUs()
        {
            ContactUsViewModel usViewModel = new ContactUsViewModel();
            var setting = _settingService.GetGeneralSiteSettingsByKey("GlobalSetting");
            usViewModel.SupportEmail = setting.SupportEmail ?? "";
            usViewModel.SupportMobile = setting.SupportMobile ?? "";
            return PartialView("_ContactUs", usViewModel);
        }

        [HttpPost]
        public IActionResult ContactUs(ContactUsViewModel usViewModel)
        {
            try
            {
                ContactUsQuery contact = new ContactUsQuery();
                contact.Name = usViewModel.FullName;
                contact.EmailAddress = usViewModel.EmailAddress;
                contact.Subject = usViewModel.Subject;
                contact.PhoneNumber = usViewModel.PhoneNumber;
                contact.Message = usViewModel.Message;
                contact.CreationOn = DateTime.UtcNow;
                contact.TicketNo = DateTime.UtcNow.Ticks;
                contact.TicketStatus = (int)TicketStatus.Pending;
                _contactService.SaveContactU(contact);
                //SaveUserNotification
                CreateNotification createNotification = new CreateNotification();
                contact.Subject = (contact.Subject != null && contact.Subject != "") ? contact.Subject : "Help & Support Ticket Raised by User";
                UserNotification userNotification = createNotification.CreateUserNotification(1,1, 4, CurrentFrontUser.Id,1, contact.Subject, contact.Message);
                _notificationService.SaveUserNotification(userNotification);
                //EndSaveUserNotification
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Request send successfully", IsSuccess = true });

            }
            catch (Exception ex)
            {

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some error occured", IsSuccess = false });
            }
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> UpdateProfileImg(IFormFile formfiles)
        {
            if (CurrentFrontUser.Id <= 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var userDtls = _userService.GetUserById(CurrentFrontUser.Id);
            if (userDtls != null && formfiles != null)
            {
                FileUpload(formfiles, SiteKeys.UploadFilesUsers);
                userDtls.ImageName = formfiles.FileName;
                await _userService.UpdateUser(userDtls);
                CurrentFrontUser.ImageName = formfiles.FileName;

                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Profile picture updated", IsSuccess = true });

            }
            else
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some error occured", IsSuccess = false });
            }
        }
        #region  [ Front User - CHANGE PASSWORD ]       

        [HttpGet]
        public IActionResult ChangePassword(long id)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.UserId = Convert.ToString(CurrentFrontUser.Id);
            return PartialView("_ChangePassword", model);
        }
        [HttpGet]
        public IActionResult IsPasswordExist(string OldPassword, string UserId)
        {
            bool isPasswordExist = false;
            var user = _userService.GetUserById(Convert.ToInt32(UserId));
            if (user != null && user.IsActive)
            {
                if (PasswordEncryption.IsPasswordMatch(user.EncryptedPassword, OldPassword, user.SaltKey))
                {
                    isPasswordExist = true;
                }
            }
            return Json(isPasswordExist);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.OldPassword == model.NewPassword)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "New password must not be equal to the old password.", IsSuccess = false });
                    }
                    var user = _userService.GetUserById(Convert.ToInt32(model.UserId));
                    if (user != null)
                    {
                        user.SaltKey = PasswordEncryption.CreateSaltKey();
                        user.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(model.NewPassword, user.SaltKey);
                        _userService.UpdateUser(user);
                        displayMsg = $"Password has been updated successfully.";
                    }
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.ToString(), IsSuccess = false });
            }
            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
        }

        #endregion   [ Front User - CHANGE PASSWORD ]  

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region MyBooking
        [HttpGet]
        public IActionResult MyBooking()
        {
            int pageSize = 3;
            int skipSize = 3;
            MyBookingViewModel myBookingViewModel = new MyBookingViewModel();
            var bookingDtls = _checkoutService.GetBookingByUserId(CurrentFrontUser.Id);
            //var bookingDtls = _checkoutService.GetBookingByUserId(12);
            List<MyBookingDto> bookings = new List<MyBookingDto>();
            if (bookingDtls != null)
            {
                bookings = BindBookingPackageData(bookingDtls);
            }
            HttpContext.Session.Set<List<MyBookingDto>>("MyBookingResultSession", bookings);
            myBookingViewModel.AllBooking = bookings;

            var upcomingBooking = bookings.Where(x => x.StartDate.Date >= DateTime.Now.Date).ToList();
            myBookingViewModel.AllBooking = bookings.OrderByDescending(o => o.BookingNo).Skip((1 - 1) * pageSize).Take(skipSize).ToList();
            myBookingViewModel.UpcomingBooking = upcomingBooking.OrderByDescending(o => o.BookingNo).Skip((1 - 1) * pageSize).Take(skipSize).ToList();
            myBookingViewModel.TotalItem = bookings.Count();
            myBookingViewModel.TotalUpcomingItem = upcomingBooking.Count();
            myBookingViewModel.CurrentPageIndex = 1;
            myBookingViewModel.CurrentPageIndexUpcoming = 1;
            return PartialView("_MyBooking", myBookingViewModel);
        }
        private List<MyBookingDto> BindBookingPackageData(List<Booking> bookings)
        {
            List<MyBookingDto> mybookingList = new List<MyBookingDto>();

            foreach (var item in bookings)
            {
                MyBookingDto myBookingDto = new MyBookingDto();
                myBookingDto.BookingNo = item.Id;
                myBookingDto.BookingPerson = item.AdultsCount.ToString() + " Adults" + (item.ChildsCount > 0 ? ", " + item.ChildsCount.ToString() + " Children" : "") + (item.InfantsCount > 0 ? ", " + item.InfantsCount.ToString() + " Infant" : "");
                myBookingDto.PackagePrice = item.TotalPrice;
                myBookingDto.PackagePriceFront = item.TotalPrice.ToString("0.##");
                var getpackage = _packageService.GetPackageDashboardById(item.PackageId);
                if (getpackage != null)
                {
                    myBookingDto.PackageId = getpackage.Id;
                    myBookingDto.PackageUrl = getpackage.PackageUrl;
                    myBookingDto.PackageName = getpackage.Name;
                    //myBookingDto.PackagePrice = item.Package.PackagePrice;
                    myBookingDto.PackageStartDate = getpackage.FromDate.ToString("dd MMM, yyyy");
                    myBookingDto.StartDate = getpackage.FromDate;
                    myBookingDto.EndDate = getpackage.ToDate;
                    myBookingDto.Description = getpackage.Description;
                    var ratingobj = _ratingReviewService.GetRatingByUserandPackageId(myBookingDto.PackageId, CurrentFrontUser.Id);
                    if (ratingobj != null)
                    {
                        myBookingDto.RatingId = ratingobj.Id;
                    }
                    //myBookingDto.PkgDesc = item.Package.Description?.Length <= 83 ? item.Package.Description : TrimHtmlText(item.Package.Description, 80);
                    myBookingDto.PkgDesc = getpackage.Description?.Length <= 203 ? getpackage.Description : TrimHtmlText(getpackage.Description, 200);
                    myBookingDto.PackageDuration = getpackage.PackageNoOfDays.ToString() + " Days & " + getpackage.PackageNoOfNights.ToString() + " Nights";


                    if (getpackage.PackageImages != null && getpackage.PackageImages.Count > 0)
                    {
                        var imagesObj = getpackage.PackageImages?.FirstOrDefault();
                        if (imagesObj != null)
                        {
                            string RetImageName = "";
                            myBookingDto.PackageImage = GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);

                        }
                        else
                        {
                            myBookingDto.PackageImage = SiteKeys.NoImagePath_Square;
                        }
                    }
                    else
                    {
                        myBookingDto.PackageImage = SiteKeys.NoImagePath_Square;
                    }
                }
                mybookingList.Add(myBookingDto);
            }

            return mybookingList;
        }
        public IActionResult BindPartialFilterPagination(int currentpage, string _type)
        {
            int pageSize = 3;
            int skipSize = 3;
            MyBookingViewModel myBookingViewModel = new MyBookingViewModel();
            List<MyBookingDto> bookings = new List<MyBookingDto>();
            bookings = HttpContext.Session.Get<List<MyBookingDto>>("MyBookingResultSession");
            if (_type == "upcoming")
            {
                var upcomingBooking = bookings.Where(x => x.StartDate.Date >= DateTime.Now.Date).ToList();

                myBookingViewModel.AllBooking = bookings.OrderByDescending(o => o.BookingNo).Skip((1 - 1) * pageSize).Take(skipSize).ToList();
                myBookingViewModel.UpcomingBooking = upcomingBooking.OrderByDescending(o => o.BookingNo).Skip((currentpage - 1) * pageSize).Take(skipSize).ToList();
                myBookingViewModel.TotalItem = bookings.Count();
                myBookingViewModel.TotalUpcomingItem = upcomingBooking.Count();
                myBookingViewModel.CurrentPageIndex = 1;
                myBookingViewModel.CurrentPageIndexUpcoming = currentpage;
            }
            else
            {
                var upcomingBooking = bookings.Where(x => x.StartDate.Date >= DateTime.Now.Date).ToList();

                myBookingViewModel.AllBooking = bookings.OrderByDescending(o => o.BookingNo).Skip((currentpage - 1) * pageSize).Take(skipSize).ToList();
                myBookingViewModel.UpcomingBooking = upcomingBooking.OrderByDescending(o => o.BookingNo).Skip((1 - 1) * pageSize).Take(skipSize).ToList();
                myBookingViewModel.TotalItem = bookings.Count();
                myBookingViewModel.TotalUpcomingItem = upcomingBooking.Count();
                myBookingViewModel.CurrentPageIndex = currentpage;
                myBookingViewModel.CurrentPageIndexUpcoming = 1;
            }


            return PartialView("~/Views/Dashboard/_MyBooking.cshtml", myBookingViewModel);
        }
        #endregion

        #region [Booking - Rating Review]
        [HttpGet]
        public IActionResult RatingReview(long packageid, long bookingid, int ratingid)
        {
            RatingReviewViewModel model = new RatingReviewViewModel();
            if (packageid > 0 && CurrentFrontUser.Id > 0)
            {
                model.PackageId = packageid;
                model.BookingId = bookingid;
                model.RatingId = ratingid;
                var ratingDtls = _ratingReviewService.GetRatingById(ratingid);
                if (ratingDtls != null)
                {
                    model = BindRatingandReviewData(ratingDtls);
                }
            }

            return PartialView("_RatingReview", model);
        }

        private RatingReviewViewModel BindRatingandReviewData(Rating entity)
        {
            RatingReviewViewModel viewRatingViewModel = new RatingReviewViewModel();
            viewRatingViewModel.RatingId = entity.Id;
            viewRatingViewModel.PackageName = entity.Package.Name;
            viewRatingViewModel.Rating = entity.RatingVal;
            var review = _ratingReviewService.GetReviewById(entity.PackageId, entity.UserId);
            if (review != null)
            {
                viewRatingViewModel.ReviewText = review.ReviewText;
                viewRatingViewModel.ReviewId = review.ReviewId;
            }

            return viewRatingViewModel;
        }

        [HttpPost]
        public async Task<IActionResult> RatingReview(RatingReviewViewModel model)
        {
            try
            {
                bool isUpdate = false;
                var ratingDtls = _ratingReviewService.GetRatingByUserandPackageId(model.PackageId, CurrentFrontUser.Id);
                if (ratingDtls != null)
                {
                    isUpdate = true;
                    ratingDtls.RatingVal = model.Rating;
                    ratingDtls.RatingOn = DateTime.UtcNow;
                }
                else
                {
                    ratingDtls = new Rating();
                    ratingDtls.RatingOn = DateTime.UtcNow;
                }
                ratingDtls.PackageId = model.PackageId;
                ratingDtls.UserId = CurrentFrontUser.Id;
                ratingDtls.RatingVal = model.Rating;
                ratingDtls = isUpdate ? await _ratingReviewService.UpdateRatingMaster(ratingDtls) : await _ratingReviewService.SaveRatingMaster(ratingDtls);

                var reviewDtls = _ratingReviewService.GetReviewById(model.PackageId, CurrentFrontUser.Id);
                if (reviewDtls != null)
                {
                    isUpdate = true;
                    reviewDtls.ReviewText = model.ReviewText;
                    reviewDtls.ReviewOn = DateTime.UtcNow;
                }
                else
                {
                    reviewDtls = new Review();
                    reviewDtls.ReviewOn = DateTime.UtcNow;
                }
                reviewDtls.PackageId = model.PackageId;
                reviewDtls.UserId = CurrentFrontUser.Id;
                reviewDtls.ReviewText = model.ReviewText ?? "";
                reviewDtls = isUpdate ? await _ratingReviewService.UpdateReview(reviewDtls) : await _ratingReviewService.SaveReview(reviewDtls);
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Rating & Review has been submitted", IsSuccess = true });

            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some error occured", IsSuccess = false });
            }
        }
        #endregion

        #region My Notifications
        [HttpGet]
        public IActionResult MyNotifications()
        {
            int pageSize = 3;
            int skipSize = 3;

            try
            {
                MyNotificationViewModel myNotificationViewModel = new MyNotificationViewModel();
                var notificationlst = _notificationService.GetNotificationByUserId(CurrentFrontUser.Id);
                List<MyNotificationDto> notifications = new List<MyNotificationDto>();


                if (notificationlst != null)
                {
                    foreach (UserNotification r in notificationlst)
                    {
                        MyNotificationDto objNoti = new MyNotificationDto();
                        var notificationsTypeName = _notificationService.GetAllNotificationType().Where(x => x.Id == r.NotificationTypeId).Select(s => s.Name).FirstOrDefault();
                        objNoti.NotificationId = r.Id;
                        objNoti.notificationsTypeName = notificationsTypeName;
                        objNoti.Title = r.Title;
                        string descriptionWithoutHtml = Regex.Replace(r.Descriptions, "<.*?>", string.Empty);
                        objNoti.Description = descriptionWithoutHtml;

                        bool fileExists = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesNotifications, r.ImageName ?? "");
                        var fullFileName = fileExists == false ? SiteKeys.NoImagePath_Square : (SiteKeys.ImageDomain + SiteKeys.UploadFilesNotifications + r.ImageName);
                        objNoti.FileName = fullFileName;

                        objNoti.CreatedDate = r.CreatedDate.ToString(SiteKeys.DateFormatWithoutTime);
                        notifications.Add(objNoti);
                    }
                }
                HttpContext.Session.Set<List<MyNotificationDto>>("MyNotificationResultSession", notifications);
                myNotificationViewModel.NotificationList = notifications.OrderByDescending(o => o.NotificationId).Skip((1 - 1) * pageSize).Take(skipSize).ToList(); ;
                myNotificationViewModel.TotalItem = notifications.Count();
                myNotificationViewModel.CurrentPageIndex = 1;
                myNotificationViewModel.PageNo = 1;
                myNotificationViewModel.TotalRecords = notifications.Count();
                if (notifications.Count() > 0)
                    myNotificationViewModel.TotalPages = (int)(notifications.Count() / 3);
                else
                    myNotificationViewModel.TotalPages = 1;
                return PartialView("_MyNotifications", myNotificationViewModel);
            }
            catch(Exception ex)
            {
                MyNotificationViewModel myNotificationViewModel = new MyNotificationViewModel();
                return PartialView("_MyNotifications", myNotificationViewModel);
            }
        }

        public IActionResult BindPartialFilterNotificationPagination(int currentpage, string _type)
        {
            int pageSize = 3;
            int skipSize = 3;

            MyNotificationViewModel myNotificationViewModel = new MyNotificationViewModel();
            var notificationlst = _notificationService.GetNotificationByUserId(CurrentFrontUser.Id);
            List<MyNotificationDto> notifications = new List<MyNotificationDto>();

            if (notificationlst != null)
            {
                foreach (UserNotification r in notificationlst)
                {
                    MyNotificationDto objNoti = new MyNotificationDto();
                    var notificationsTypeName = _notificationService.GetAllNotificationType().Where(x => x.Id == r.NotificationTypeId).Select(s => s.Name).FirstOrDefault();
                    objNoti.NotificationId = r.Id;
                    objNoti.notificationsTypeName = notificationsTypeName;
                    objNoti.Title = r.Title;
                    string descriptionWithoutHtml = Regex.Replace(r.Descriptions, "<.*?>", string.Empty);
                    objNoti.Description = descriptionWithoutHtml;

                    bool fileExists = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesNotifications, r.ImageName ?? "");
                    var fullFileName = fileExists == false ? SiteKeys.NoImagePath_Square : (SiteKeys.ImageDomain + SiteKeys.UploadFilesNotifications + r.ImageName);
                    objNoti.FileName =  fullFileName;

                    objNoti.CreatedDate = r.CreatedDate.ToString(SiteKeys.DateFormatWithoutTime);
                    notifications.Add(objNoti);
                }
            }
            HttpContext.Session.Set<List<MyNotificationDto>>("MyNotificationResultSession", notifications);
            myNotificationViewModel.NotificationList = notifications.OrderByDescending(o => o.NotificationId).Skip((currentpage - 1) * pageSize).Take(skipSize).ToList(); ;
            myNotificationViewModel.TotalItem = notifications.Count();
            myNotificationViewModel.CurrentPageIndex = currentpage;

            myNotificationViewModel.PageNo = currentpage;
            myNotificationViewModel.TotalRecords = notifications.Count();
            if (notifications.Count() > 0)
                myNotificationViewModel.TotalPages = (int)(notifications.Count() / 3);
            else
                myNotificationViewModel.TotalPages = 1;
            return PartialView("~/Views/Dashboard/_MyNotifications.cshtml", myNotificationViewModel);
        }

        [HttpGet]
        public IActionResult GetNotificationDetails(int? notificationid)
        {
            MyNotificationDto model = new MyNotificationDto();
            model.NotificationId = notificationid ?? 0;
            if (notificationid.HasValue)
            {
                
                var accObj = _notificationService.GetUserNotificationById(notificationid.Value);
                if (accObj != null)
                {
                    var notificationsTypeName = _notificationService.GetAllNotificationType().Where(x => x.Id == accObj.NotificationTypeId).Select(s => s.Name).FirstOrDefault();
                    model.Title = accObj.Title;
                    model.notificationsTypeName = notificationsTypeName;
                    string descriptionWithoutHtml = Regex.Replace(accObj.Descriptions, "<.*?>", string.Empty);
                    model.Description = descriptionWithoutHtml;
                    bool fileExists = CommonFileViewModel.CheckIfFileExistsForDestination(SiteKeys.UploadFilesNotifications, accObj.ImageName ?? "");
                    var fullFileName = fileExists == false ? SiteKeys.NoImagePath_Square : (SiteKeys.ImageDomain + SiteKeys.UploadFilesNotifications + accObj.ImageName);
                    model.FileName = fullFileName;
                    model.CreatedDate = accObj.CreatedDate.ToString(SiteKeys.DateFormatWithoutTime);
                    //--- update notification isVisited status----
                    if (accObj.IsVisited == false)
                    {
                        accObj.IsVisited = true;
                        _notificationService.UpdateUserNotification(accObj);
                    }
                    //-----------------------------------
                }
            }
            return PartialView("_NotificationDetails", model);
        }

        [HttpGet]
        public string GetNotificationCount()
        {
            var notificationlst = _notificationService.GetNotificationByUserId(CurrentFrontUser.Id);
            string totalUnreadNotification = "0";
            if(notificationlst != null)
            {
                notificationlst =notificationlst.Where(x=>x.IsVisited==false).ToList();
                totalUnreadNotification = notificationlst.Count.ToString();
            }
            return totalUnreadNotification;
        }

        #endregion
    }

}