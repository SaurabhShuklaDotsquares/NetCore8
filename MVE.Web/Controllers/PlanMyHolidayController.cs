using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NuGet.Configuration;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Security.Principal;
using TCP.Core;
using TCP.Core.Code.LIBS;
using TCP.Core.Code.Notification;
using TCP.Core.Models;
using TCP.Data.Models;
using TCP.Dto;
using TCP.Service;
using TCP.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCP.Web.Controllers
{
    public class PlanMyHolidayController : BaseController
    {
        private readonly IConfiguration _configuration;
        private IDataProtector _protector;
        private readonly IUserService _userService;
        private readonly IPlanHolidayService _planHolidayService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly TravelCustomPackagesContext _context;
        private readonly ICountryService _countryService;
        private readonly INotificationService _notificationService;

        private readonly ISettingService _settingService;
        public PlanMyHolidayController(INotificationService notificationService, IConfiguration configuration, IDataProtectionProvider provider, IUserService userService, IPlanHolidayService planHolidayService, IEmailFactoryService emailFactoryService, TravelCustomPackagesContext context, ICountryService countryService, ISettingService settingService)
        {
            _configuration = configuration;
            var protectorPurpose = "secure username and password";
            _protector = provider.CreateProtector(protectorPurpose);
            _userService = userService;
            _planHolidayService = planHolidayService;
            _emailFactoryService = emailFactoryService;
            _context = context;
            _countryService = countryService;
            _notificationService = notificationService;
            _settingService = settingService;
        }


        #region [PlanMyHoliday]
        [HttpGet]
        public async Task<IActionResult> PlanMyHoliday()
        {
            //await _planHolidayService.GetHolidayQuestionOption();

            PlanMyHolidayViewModel planMyObj = new PlanMyHolidayViewModel();
            List<QuestionsModel> queslst = new List<QuestionsModel>();


            var questionAndOptionsList = _context.HolidayQuestionAndOptions.Where(x => x.IsActive == true).OrderBy(x => x.OrderIndex).ToList();
            foreach (var quesitem in questionAndOptionsList.Where(x => x.IsOption == false))
            {
                List<OptionsModel> optionlst = new List<OptionsModel>();
                QuestionsModel quesObj = new QuestionsModel();

                quesObj.QuestionOrOptionName = quesitem.QuestionOrOptionName;
                quesObj.IFieldRequired = quesitem.IfieldRequired ?? false;
                quesObj.QueOptionDescription = quesitem.OptionDescription?.Trim();
                quesObj.QuesKey = quesitem.QuestionKey?.Trim();
                quesObj.SectionId = quesitem.SectionId ?? 0;
                foreach (var option in questionAndOptionsList.Where(x => x.ParentId == quesitem.Id && x.IsOption == true).ToList())
                {
                    OptionsModel optionsModel = new OptionsModel();
                    optionsModel.Id = option.OptionIndex ?? 0;
                    optionsModel.Option = option.QuestionOrOptionName?.Trim();
                    optionsModel.OptionDescription = option.OptionDescription?.Trim();
                    optionsModel.QuesKey = option.QuestionKey?.Trim();
                    optionlst.Add(optionsModel);
                }

                quesObj.Optionslist = optionlst;
                queslst.Add(quesObj);
            }
            planMyObj.Questionslist = queslst;

            ViewBag.NoOfTravellers = CommonFileViewModel.GetNoOfTravellers();
            ViewBag.Months = GetAllMonths();
            planMyObj.DestinationList = _countryService.GetCountryMastersForDropDownWithShortURL().OrderBy(o => o.Text).ToList();

            //TimeZone
            List<SelectListItem> timezoneList = new List<SelectListItem>();

            timezoneList.Add(new SelectListItem() { Value = "", Text = "Select Time zone...", Selected = false });

            var timezoneInfo = TimeZoneInfo.GetSystemTimeZones();

            foreach (var item in timezoneInfo)
            {
                timezoneList.Add(new SelectListItem()
                {
                    Value = item.StandardName,
                    Text = item.DisplayName,
                    Selected = false
                });
            }
            planMyObj.TimezoneList = timezoneList;

            var globalSetting = GetGlobalSetting();
            planMyObj.SupportEmail = globalSetting.SupportEmail;
            planMyObj.SupportMobile = globalSetting.SupportMobile;
            planMyObj.LogoImageName = globalSetting.LogoImageName;
            planMyObj.LogoImageNameDark = globalSetting.LogoImageNameDark;

            return PartialView("_PlanMyHoliday", planMyObj);
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
                obj.LogoImageName = SiteKeys.Domain + "/Image" + SiteKeys.LogoImageName;
                obj.LogoImageNameDark = SiteKeys.Domain + "/Image" + SiteKeys.LogoImageNameDark;
            }
            return obj;
        }


        private dynamic GetAllMonths()
        {
            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;

            // Print the names of all the months
            foreach (string monthName in monthNames)
            {
                Console.WriteLine(monthName);
            }
            return monthNames;
        }

        [HttpPost]

        public async Task<IActionResult> PlanMyHoliday(PlanMyHolidayViewModel modal)
        {
            try
            {
                bool isNewUser = false;
                string displayMsg = string.Empty;
                string WhenUserActive = string.Empty;
                string link_WhenUserActive = string.Empty;
                #region Create New USER
                RegisterViewModel registerView = new RegisterViewModel();
                registerView.Email = modal.Email;
                registerView.MobilePhone = modal.PhoneNumber;
                User usermodel = _userService.GetUserByEmail(registerView.Email);
                if (usermodel == null && _userService.IsUserExists(modal.Email) == false)
                {
                    usermodel = CommonFileViewModel.RegisterNewUser(registerView);
                    usermodel = await _userService.SaveUser(usermodel);
                    Billing billing = _userService.GetBillingByUserId(usermodel.Id) ?? new Billing();
                    if (billing?.UserId <= 0)
                    {
                        billing.UserId = usermodel.Id;
                        billing.CreatedBy = CurrentFrontUser.Id;
                        billing.CreationOn = DateTime.UtcNow;
                        await _userService.SaveBilling(billing);
                    }
                    isNewUser = true;   
                }
                else
                {
                    usermodel.MobilePhone = modal.PhoneNumber;
                    usermodel = await _userService.UpdateUser(usermodel);
                }
                #endregion

                #region NewPackageRequest saving with user id

                NewPackageRequest newReq = new NewPackageRequest();
                if (usermodel != null && usermodel.Id > 0)
                {
                    newReq.UserId = usermodel.Id;
                    newReq.DestinationFrom = modal.From?.Trim();
                    newReq.DestinationTo = modal.To?.Trim();
                    if (!string.IsNullOrEmpty(modal.DepartureDateType) && modal.DepartureDateType == DepartureDateType.Fixed.ToString())
                    {
                        //newReq.DepartureDate = !string.IsNullOrEmpty(modal.DepartureDate) ? Convert.ToDateTime(modal.DepartureDate) : DateTime.Now;
                        newReq.DepartureDate = !string.IsNullOrEmpty(modal.DepartureDate) ? DateTime.ParseExact(modal.DepartureDate, "dd/MM/yyyy", CultureInfo.InvariantCulture) : DateTime.Now;
                        newReq.DepartureDateType = modal.DepartureDateType;
                    }
                    else if (!string.IsNullOrEmpty(modal.DepartureDateType) && modal.DepartureDateType == DepartureDateType.Flexible.ToString())
                    {
                        newReq.DepartureMonth = modal.DepartureMonth;
                        newReq.DepartureWeek = modal.DepartureWeek + " Week";
                        newReq.DepartureDateType = modal.DepartureDateType;

                    }
                    else if (!string.IsNullOrEmpty(modal.DepartureDateType) && modal.DepartureDateType == DepartureDateType.Anytime.ToString())
                    {
                        newReq.DepartureDays = modal.DepartureDays + " Days";
                        newReq.DepartureDateType = modal.DepartureDateType;

                    }

                    newReq.Region = "";
                    newReq.Email = modal.Email;
                    newReq.Phone = modal.PhoneNumber;
                    newReq.HotelCategoryRating = modal.HotelCategoryRating ?? "";
                    newReq.AccommodationType = modal.Accommodations ?? "";
                    newReq.IsDomesticFlights = modal.DomesticFlights?.ToUpper() == "YES" ? true : false;
                    newReq.PerHeadBudgetWithoutAirfair = modal.BudgetWithoutAirfare;

                    newReq.Adults = modal.Adults;
                    newReq.Infants = modal.Infants;
                    newReq.Children = modal.Children;
                    newReq.IwillBookIn = !string.IsNullOrEmpty(modal.BookWhen) ? modal.BookWhen : "In Next 2-3 Days";
                    newReq.IsReqCabForLocalSite = modal.CabForLocalSightseeing?.ToUpper() == "YES" ? true : false;
                    newReq.TripType = modal.TypeOfTrip??"";
                    newReq.PreferredTimeToCallId = modal.PreferredTimeToCallYou??"";
                    newReq.TimeZoneForPrefTimeToCall = modal.InputTimeZone;
                    newReq.YourAgeRange = modal.YourAge??"";
                    newReq.RequiredInPackage = modal.RequiredInYourLandPackage ?? "";

                        newReq.IsAdditionalRequirement = true;
                        newReq.SpecialConsideration = modal.SpecialConsiderations?.Trim();
                        newReq.ArrivalDepartueDateIfTktBooked = modal.ArrivalAndDepartureDateTime?.Trim();
                        newReq.NewPackageRequestStatusId = (int)PackageRequestStatus.Pending;
                        newReq.HotelChoiceIfAny = modal.HotelChoiceIfAny?.Trim();
                        newReq.OtherAdditional = modal.Other?.Trim();
                        newReq.IsDeleted = false;
                        newReq.IsActive = true;
                        newReq.CreatedOn = DateTime.UtcNow;
                        newReq.CreatedBy = CurrentFrontUser.Id;
                        newReq = await _planHolidayService.SaveNewPackageRequest(newReq); //NewPackageRequest saving

                    #endregion

                    #region Send Email to user based on above ids for login  
                    if (newReq != null && newReq.Id > 0)
                    {                       
                        registerView.Password = string.IsNullOrEmpty(HttpContext.Session.GetString("RegisteredPwd")) ? "" : HttpContext.Session.GetString("RegisteredPwd");

                        if (string.IsNullOrEmpty(registerView.Password) || string.IsNullOrWhiteSpace(registerView.Password))
                        {
                            usermodel.SaltKey = PasswordEncryption.CreateSaltKey();
                            var password = PasswordEncryption.GenerateRandomPassword();

                            usermodel.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(password, usermodel.SaltKey);
                            await _userService.UpdateUser(usermodel);
                            registerView.Password = password;
                        }

                        WhenUserActive = usermodel.IsActive == false ? $" if you didn't receive any email? then please click on <span class='text-primary resendMailClass cursor-pointer'  data-href='/PlanMyHoliday/ResendPlanConfirmationLink?userid={usermodel.Id}&reqId={newReq.Id}&regPwd={registerView.Password}'>  Resend Confirmation Link</span>" : "";
                        link_WhenUserActive = usermodel.IsActive == false ? $" you will get a confirmation link on your registered email <b>{usermodel.Email}</b> in short while, please click on the link to activate your sign up request.{WhenUserActive}" : "your request has been sent to our sales executive, they will contact you soon on your email or phone.";

                        await Task.Run(() => { _emailFactoryService.SendCustomPlanEmail(usermodel, newReq.Id, 'N', registerView.Password, isNewUser); });
                        await _userService.UpdateUser(usermodel);

                        displayMsg = $"<b>Thank you for submitting your request.</b><br/> {link_WhenUserActive}";

                        //SaveUserNotification
                        CreateNotification createNotification = new CreateNotification();
                        UserNotification userNotification = createNotification.CreateUserNotification(1,1, 5, CurrentFrontUser.Id,1, "New plan my holiday enquiry submitted", "New plan my holiday enquiry id: " + newReq.Id.ToString() + " submitted successfully.");
                        await _notificationService.SaveUserNotification(userNotification);
                        //EndSaveUserNotification

                    }
                    #endregion
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });

                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "User doesn't exists", IsSuccess = false });
                }



            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = ex.Message, IsSuccess = false });
            }
        }

        public async Task<IActionResult> ResendPlanConfirmationLink(int userid, int reqId, string regPwd)
        {
            string displayMsg = string.Empty;
            try
            {
                if (userid <= 0 && reqId <= 0 && regPwd == null)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Something went wrong", IsSuccess = false });

                }
                var usermodel = _userService.GetUserById(userid);
               
                if (usermodel != null && usermodel.IsActive == false && usermodel.IsDeleted == false)
                {
                    if (usermodel.Id > 0 && usermodel.IsEmailVerified == false)
                    {
                        usermodel.EmailVerificationToken = Guid.NewGuid().ToString();
                        usermodel.EmailVerificationOtpExpired = DateTime.Now.AddDays(30);
                        await _userService.UpdateUser(usermodel);
                        await Task.Run(() => { _emailFactoryService.SendCustomPlanEmail(usermodel, reqId, 'C', regPwd, true); });
                        usermodel.ModifiedOn = DateTime.Now;
                        await _userService.UpdateUser(usermodel);
                    }
                    displayMsg = $"<b>Thank you for submitting your request.</b> A confirmation link has been resent to your email {usermodel.Email}.Please click on the link to activate your request." +
                                $"Did't receive email? <span class='text-primary resendMailClass cursor-pointer' data-href='/PlanMyHoliday/ResendPlanConfirmationLink?userid={userid}&reqId={reqId}&regPwd={regPwd}'> Resend Confirmation Link</span>" ;
                    //displayMsg = $"Thank you for submitting your request. A confirmation link has been resent to your email {usermodel.Email}.";
                    //ShowSuccessMessage("Success", displayMsg, false);
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { Message = displayMsg, IsSuccess = true });
                }
                else if (usermodel.IsActive == true && usermodel.IsDeleted == false)
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Your request has been confirmed already, Please Login", IsSuccess = false });
                }
                else
                {
                    return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "User does't exists , Please Register", IsSuccess = false });
                    //// ShowErrorMessage("Error", "User does't exists , Please Register", false);
                }
            }
            catch (Exception ex)
            {
                return NewtonSoftJsonResult(new RequestOutcome<dynamic> { ErrorMessage = "Some Error Occurred please try after some time", IsSuccess = false });
                ////ShowErrorMessage("Error", "Some Error Occurred please try after some time.", false);
            }           
        }
        #endregion


    }
}
