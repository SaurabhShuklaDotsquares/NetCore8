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
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;
using TCP.Web.CommonClass;

namespace TCP.Web.Controllers
{
    public class MyDashboardController : BaseController
    {
        private readonly IPackageService _packageService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICheckoutService _checkoutService;
        private readonly IUserService _userService;

        public MyDashboardController(IHostingEnvironment hostingEnvironment, IPackageService packageService, ICheckoutService checkoutService, IUserService userService)
        {
            _packageService = packageService;
            _hostingEnvironment = hostingEnvironment;
            _checkoutService = checkoutService;
            _userService = userService;

        }
        public IActionResult Index()
        {
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Login Please", false);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public IActionResult BindPartialData(string _type)
        {
            if (_type == "mybooking")
            {
                int pageSize = 3;
                int skipSize = 3;
                MyBookingViewModel myBookingViewModel = new MyBookingViewModel();
                var bookingDtls = _checkoutService.GetBookingByUserId(CurrentFrontUser.Id);
                //var bookingDtls = _checkoutService.GetBookingByUserId(10);
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
                return PartialView("~/Views/MyDashboard/_MyBooking.cshtml", myBookingViewModel);
            }
            return PartialView("~/Views/MyDashboard/_Dashboard.cshtml");
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


            return PartialView("~/Views/MyDashboard/_MyBooking.cshtml", myBookingViewModel);
        }
        private List<MyBookingDto> BindBookingPackageData(List<Booking> bookings)
        {
            List<MyBookingDto> mybookingList = new List<MyBookingDto>();

            foreach (var item in bookings)
            {
                MyBookingDto myBookingDto = new MyBookingDto();
                myBookingDto.BookingNo = item.Id;
                myBookingDto.BookingPerson = item.AdultsCount.ToString() + " Adults" + (item.ChildsCount > 0 ? ", " + item.ChildsCount.ToString() + " Children" : "") + (item.InfantsCount > 0 ? ", " + item.InfantsCount.ToString() + " Infant" : "");
                if (item.Package != null)
                {
                    myBookingDto.PackageId = item.Package.Id;
                    myBookingDto.PackageName = item.Package.Name;
                    myBookingDto.PackagePrice = item.Package.PackagePrice;
                    myBookingDto.PackageStartDate = item.Package.FromDate.ToString("dd MMM, yyyy");
                    myBookingDto.StartDate = item.Package.FromDate;
                    myBookingDto.Description = item.Package.Description;

                    myBookingDto.PkgDesc = item.Package.Description?.Length <= 83 ? item.Package.Description : TrimHtmlText(item.Package.Description, 80);
                    myBookingDto.PackageDuration = item.Package.PackageNoOfDays.ToString() + " Days & " + item.Package.PackageNoOfNights.ToString() + " Nights";


                    var getpackage = _packageService.GetActivePackageById(item.PackageId);
                    var imagesObj = getpackage.PackageImages?.FirstOrDefault();
                    if (imagesObj != null)
                    {
                        string RetImageName = "";
                        myBookingDto.PackageImage = GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);

                    }
                }
                mybookingList.Add(myBookingDto);
            }

            return mybookingList;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.UserId = Convert.ToString(CurrentFrontUser.Id);
            return PartialView("~/Views/MyDashboard/_ChangePassword.cshtml", model);
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            string displayMsg = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
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
    }
}
