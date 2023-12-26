using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using TCP.Core;
using TCP.Core.Code.LIBS;
using TCP.Core.Code.Notification;
using TCP.Data.Models;
using TCP.Dto;
using TCP.Service;
using TCP.Web.ViewModels;

namespace TCP.Web.Controllers
{
    public class CheckoutController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly IUserService _userService;
        private readonly TravelCustomPackagesContext _context;
        private readonly ICheckoutService _checkoutservice;
        private readonly IPackageService _pkgservice;
        private readonly INotificationService _notificationService;

        public CheckoutController(INotificationService notificationService, ICountryService countryService, IUserService userService, IEmailFactoryService emailFactoryService, TravelCustomPackagesContext context, ICheckoutService checkoutservice, IPackageService pkgservice)
        {
            _countryService = countryService;
            _userService = userService;
            _emailFactoryService = emailFactoryService;
            _context = context;
            _checkoutservice = checkoutservice;
            _pkgservice = pkgservice;
            _notificationService = notificationService;

        }
        // Booking Work
        public IActionResult Index(string PackageUrl)
        {
            int? packageId = 0;
            string package_url = "/package/" + PackageUrl;
            int lastSlashIndex = package_url.LastIndexOf('/');
            if (lastSlashIndex != -1)
            {
                // Get the substring from the last slash to the end
                string desiredPart = package_url.Substring(lastSlashIndex + 1);
                packageId = Convert.ToInt32(_pkgservice.GetActivePackageByUrl(desiredPart).Id);
            }
            
            if (CurrentFrontUser.Id <= 0)
            {
                ShowErrorMessage("Error", "Please log in first to make a booking.", false);

                //return RedirectToAction("Index", "ListingDetail", new { package_url = PackageUrl });
               
                return Redirect(package_url);
            }
            User usermodel = _userService.GetUserByEmail(CurrentFrontUser.Email);


            if (usermodel == null || _userService.IsUserExists(CurrentFrontUser.Email) == false)
            {
                ShowErrorMessage("Error", "User does't exists to this email.", false);
                return Redirect(package_url);
            }

            PackageViewModel vm = new PackageViewModel();
            vm.Id = packageId ?? 0;
            //Package packages = _pkgservice.GetActivePackageById(Convert.ToInt64(vm.Id));
            Package packages = _pkgservice.GetActivePackageFutureDateById(Convert.ToInt64(vm.Id));
            if (packages == null)
            {
                ShowErrorMessage("Error", "This Package is not active, Please contact to administrator.", false);

                return Redirect(package_url);
            }
            vm.SetEntity(packages);
            vm.ComposeViewData();
            if (packages != null)
            {
                vm.CountryName = packages.Country?.Name ?? "";
                vm.FromDate = packages.FromDate.ToString("dd MMM, yyyy");
                vm.ToDate = packages.ToDate.ToString("dd MMM, yyyy");
                vm.PackagePrice = packages.PackagePrice;
                vm.PackagePriceFront = packages.PackagePrice.ToString("0.##");
            }
            vm.siteSettings = _context.GeneralSiteSettings.Where(x => x.KeyName == "GlobalSetting").FirstOrDefault();

            TravellersViewModel bookView = new TravellersViewModel();
            bookView.EmailAddress = usermodel.Email;
            bookView.MobilePhone = usermodel.MobilePhone;
            bookView.City = usermodel.City;
            bookView.ZipCode = usermodel.ZipCode;
            bookView.Address = usermodel.Address;
            bookView.CountryId = usermodel.CountryId ?? 0;
            bookView.FullName = CommonFileViewModel.GetFullName(usermodel.FirstName ?? "", usermodel.LastName ?? "");
            vm.travellersmodel = bookView;

            #region [Bind Select List Items]

            ViewBag.NoOfTravellers = CommonFileViewModel.GetNoOfTravellers();

            vm.travellersmodel.CountyList = _countryService.GetAllCountries(true, false).Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}", Selected = c.Id == bookView.CountryId }).ToList();
            vm.travellersmodel.CountyList.Insert(0, new SelectListItem() { Text = "-- Select --" });
            #endregion
            //Calculation
            var _totalPrice = vm.PackagePrice;

            var _taxPercent1 = vm.siteSettings?.ApplyTaxPercent1;
            //GST
            decimal GSTAmount = 0, TCSAmount = 0, NetAmount = 0;
            GSTAmount = ((_totalPrice * _taxPercent1.Value) / 100);
            ViewBag.spTaxTotal1 = GSTAmount.ToString("0.##");

            var _taxPercent2 = vm.siteSettings?.ApplyTaxPercent2;
            //TCS
            TCSAmount = ((_totalPrice * _taxPercent2.Value) / 100);
            ViewBag.spTaxTotal2 = TCSAmount.ToString("0.##");

            NetAmount = _totalPrice + GSTAmount + TCSAmount;
            ViewBag.spGrandTotal = NetAmount.ToString("0.##");
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PackageViewModel packageobj)
        {
            try
            {
                // Package packages = _pkgservice.GetActivePackageById(packageobj.Id);
                Package packages = _pkgservice.GetActivePackageFutureDateById(packageobj.Id);
                if (packages == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "This Package is not active, Please contact to administrator.", IsSuccess = false });

                }
                var generalSiteSetting = _context.GeneralSiteSettings.Where(x => x.KeyName == "GlobalSetting").FirstOrDefault();
                if (generalSiteSetting != null && packageobj != null && packageobj.travellersmodel != null)
                {

                    int totaladult = packageobj.NoOfAdults;
                    int totalChilds = packageobj.NoOfChilds;
                    int totalAdultsChild = totaladult + totalChilds;
                    decimal _totalPrice = packageobj.PackagePrice * totalAdultsChild;
                    decimal _NetAmount = 0;

                    var _siteSettings = _context.GeneralSiteSettings.Where(x => x.KeyName == "GlobalSetting").FirstOrDefault();

                    //Calculation
                    var _taxPercent1 = _siteSettings?.ApplyTaxPercent1;
                    //GST
                    decimal GSTAmount = ((_totalPrice * _taxPercent1.Value) / 100);

                    var _taxPercent2 = _siteSettings?.ApplyTaxPercent2;
                    //TCS
                    decimal TCSAmount = ((_totalPrice * _taxPercent2.Value) / 100);

                    decimal NetAmount = _totalPrice + GSTAmount + TCSAmount;
                    _NetAmount = NetAmount;

                    Booking booking = new Booking();
                    booking.PackageId = packageobj.Id;
                    booking.UserId = CurrentFrontUser.Id; //userid
                    booking.FirstName = packageobj.travellersmodel.FirstName?.Trim() ?? packageobj.travellersmodel.FullName;
                    booking.LastName = packageobj.travellersmodel.LastName?.Trim() ?? "";
                    booking.Email = packageobj.travellersmodel.EmailAddress;
                    booking.MobilePhone = packageobj.travellersmodel.MobilePhone;
                    booking.CountryCode = packageobj.travellersmodel.CountryCode;
                    booking.BookingFor = packageobj.travellersmodel.BookingFor;
                    booking.TotalPrice = _NetAmount;

                    booking.AdultsCount = packageobj.NoOfAdults;
                    booking.InfantsCount = packageobj.NoOfInfants;
                    booking.ChildsCount = packageobj.NoOfChilds;
                    booking.PassportNo = packageobj.travellersmodel.PassportNo;
                    booking.SpecialRequests = packageobj.travellersmodel.SpecialRequests;

                    booking.ApplyTaxPercent1 = generalSiteSetting.ApplyTaxPercent1;
                    booking.ApplyTaxPercentHeading1 = generalSiteSetting.ApplyTaxPercentHeading1;
                    booking.ApplyTaxPercent2 = generalSiteSetting.ApplyTaxPercent2;
                    booking.ApplyTaxPercentHeading2 = generalSiteSetting.ApplyTaxPercentHeading2;
                    booking.AppliedDiscountPercent = generalSiteSetting.DiscountPercent;
                    booking.AppliedFixedDiscunt = generalSiteSetting.DiscountFix;
                    booking.CountryId = packageobj.travellersmodel.CountryId;
                    booking.StateId = packageobj.travellersmodel.StateId;
                    if (packageobj?.travellersmodel?.StateId == 0)
                    {
                        return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Please select state", IsSuccess = false });
                    }
                    booking.City = packageobj.travellersmodel.City;
                    booking.ZipCode = packageobj.travellersmodel.ZipCode;
                    booking.Address = packageobj.travellersmodel.Address;
                    booking.BookingStatusId = (int)BookingStatus.Pending;
                    booking.CreationOn = DateTime.UtcNow;
                    booking.CreatedBy = CurrentFrontUser.Id;
                    booking = await _checkoutservice.SaveBooking(booking);



                    var payAmount = _NetAmount * 100;


                    #region Payment getway
                    if (CurrentFrontUser.Id > 0)
                    {
                        try
                        {
                            StripeConfiguration.ApiKey = SiteKeys.Stripekey;
                            Guid guid = Guid.NewGuid();
                            var checkout_options = new SessionCreateOptions
                            {
                                SuccessUrl = SiteKeys.Domain + "/Checkout/PaymentSuccess?token=" + guid.ToString(),
                                CancelUrl = SiteKeys.Domain + "/Checkout/PaymentFailed?token=" + guid.ToString(),
                                LineItems = new List<SessionLineItemOptions>
                        {
                            new SessionLineItemOptions
                            {
                                PriceData=new SessionLineItemPriceDataOptions
                                {
                                    UnitAmount= (long)payAmount,
                                    Currency=SiteKeys.StripeCurrency,
                                    ProductData=new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name=packages.Name,
                                    }
                                },
                                Quantity = 1,
                            },
                        },
                                CustomerCreation = "always",
                                Mode = "payment"
                            };
                            var checkout_service = new SessionService();
                            var check_outs = checkout_service.Create(checkout_options);



                            if (booking != null)
                            {
                                Transaction transaction = new Transaction();
                                transaction.BookingId = booking.Id;
                                transaction.PaymentAmt = _NetAmount;
                                transaction.PaymentDateTime = DateTime.UtcNow;
                                transaction.PaymentMethodId = 1;
                                transaction.TransactionStatusId = (int)BookingStatus.Pending;
                                transaction.PmtGatewayResponseData = check_outs.ToJson();
                                transaction.CreationOn = DateTime.UtcNow;
                                transaction.CreatedBy = CurrentFrontUser.Id;
                                transaction.CreatedBy = CurrentFrontUser.Id;
                                transaction.TransactionId = check_outs.Id;
                                transaction.PaymentType = "card";
                                transaction.PaymentToken = guid.ToString();
                                transaction = await _checkoutservice.SaveTransaction(transaction);
                            }
                            //SaveUserNotification
                            CreateNotification createNotification = new CreateNotification();
                            UserNotification userNotification = createNotification.CreateUserNotification(1, 1, 3, CurrentFrontUser.Id, 1, "Package Booking", "Bookig of package id " + packages.Id + " has been successfull.");
                            await _notificationService.SaveUserNotification(userNotification);
                            //EndSaveUserNotification
                            var payUrl = ((Newtonsoft.Json.Linq.JValue)check_outs.RawJObject["url"]).Value.ToString();
                            return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Booked Successfully", RedirectUrl = payUrl, IsSuccess = true });

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    #endregion


                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = "Booked Successfully", IsSuccess = true });

                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Setting keys or Travellers details are null", IsSuccess = false });
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.Message, IsSuccess = false });
            }

        }
        [HttpGet]
        public IActionResult GetStates(int country)
        {
            var states = _context.StateMasters.Where(x => x.CountryId == country).Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}" }).ToList();
            return Json(states);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(string token)
        {
            PaymentSuccessViewModel transactionSuccessDto = new PaymentSuccessViewModel();
            Transaction transactions = _checkoutservice.GetTransactionByToken(token);
            if (transactions != null)
            {
                transactionSuccessDto.BookingId = transactions.BookingId;
                transactionSuccessDto.TransactionId = transactions.TransactionId;
                transactionSuccessDto.Date = transactions.PaymentDateTime.ToString("dd/MM/yyyy HH:mm");
                transactionSuccessDto.Message = "Congratulation your Booking and payment has been done successfully.";

                //Check Status
                StripeConfiguration.ApiKey = SiteKeys.Stripekey;

                var service = new SessionService();
                var result = service.Get(transactions.TransactionId);
                var pstatus = result.Status;
                var paymentStatus = result.PaymentStatus;
                decimal paymentAmount = Convert.ToDecimal(result.AmountTotal) / 100;
                if (paymentStatus != "paid")
                {
                    transactionSuccessDto.Message = "Your payment has failed, please rebook.";
                    return View(transactionSuccessDto);
                }

                transactions.PaymentAmt = paymentAmount;
                transactions.TransactionStatusId = (int)BookingStatus.Confirmed;
                transactions.ModifiedOn = DateTime.UtcNow;
                transactions.ModifiedBy = CurrentFrontUser.Id;
                transactions = await _checkoutservice.UpdateTransaction(transactions);

                Booking bookings = _checkoutservice.GetBookingDetailsById(transactions.BookingId);
                if (bookings != null)
                {
                    bookings.BookingStatusId = (int)BookingStatus.Confirmed;
                    bookings.ModifiedOn = DateTime.UtcNow;
                    bookings.ModifiedBy = CurrentFrontUser.Id;
                    var book = await _checkoutservice.UpdateBooking(bookings);
                }

                //Email
                BookingSuccessDto bookingSuccessDto = new BookingSuccessDto();
                bookingSuccessDto.BookingId = transactions.BookingId;
                bookingSuccessDto.TransactionId = transactions.TransactionId;
                bookingSuccessDto.BookingDate = transactions.PaymentDateTime.ToString("dd/MM/yyyy HH:mm");

                bookingSuccessDto.FromDate = bookings.Package != null ? bookings.Package.FromDate.ToString("dd/MM/yyyy") : string.Empty;
                bookingSuccessDto.Todate = bookings.Package != null ? bookings.Package.ToDate.ToString("dd/MM/yyyy") : string.Empty;
                bookingSuccessDto.PackageName = bookings.Package != null ? bookings.Package.Name : string.Empty;
                bookingSuccessDto.FirstName = bookings.User != null ? bookings.User.FirstName : string.Empty;
                bookingSuccessDto.LastName = bookings.User != null ? bookings.User.LastName : string.Empty;
                bookingSuccessDto.Email = bookings.User != null ? bookings.User.Email : string.Empty;
                bookingSuccessDto.PackageId = bookings.PackageId.ToString();
                bookingSuccessDto.PackageUrl = bookings.Package != null ? bookings.Package.PackageUrl : string.Empty;

                await Task.Run(() => { _emailFactoryService.SendEmailBookingSuccess(bookingSuccessDto); });
            }

            return View(transactionSuccessDto);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentFailed(string token)
        {
            Transaction transactions = _checkoutservice.GetTransactionByToken(token);
            if (transactions != null)
            {
                transactions.TransactionStatusId = (int)BookingStatus.Cancelled;
                transactions.ModifiedOn = DateTime.UtcNow;
                transactions.ModifiedBy = CurrentFrontUser.Id;
                transactions = await _checkoutservice.UpdateTransaction(transactions);

                Booking bookings = _checkoutservice.GetBookingDetailsById(transactions.BookingId);
                if (bookings != null)
                {
                    bookings.BookingStatusId = (int)BookingStatus.Cancelled;
                    bookings.ModifiedOn = DateTime.UtcNow;
                    bookings.ModifiedBy = CurrentFrontUser.Id;
                    var book = await _checkoutservice.UpdateBooking(bookings);
                }
            }

            return View();
        }
    }
}
