using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TCP.Core.Models.Security;

namespace TCP.Web.ViewModels
{
    public class PlanMyHolidayViewModel 
    {
        #region [ Private Properties ]
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private CustomPrincipal CurrentUser { get; set; }

        public void SetEntity(CustomPrincipal currentUser)
        {
            CurrentUser = currentUser;
        }

        public void SetHostingEnvironment(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }
        #endregion

        [Required(ErrorMessage = "Please enter Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Phone Number")]
        [MaxLength(10)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Enter valid Phone Number and length should be 10 to 16 characters")]
        public string PhoneNumber { get; set; }
        //public DateTime DestinationFrom { get; set; }
        //public DateTime DestinationTo { get; set; }
        
        public List<QuestionsModel> Questionslist { get; set; }

        //--------------------------------------------
        public string From { get; set; }
        public string To { get; set; }
        [Required(ErrorMessage = "Please select any Departure")]
        public string DepartureDateType { get; set; }
        //public string Flexible { get; set; }
        //public string Anytime { get; set; }
        public string DepartureDate { get; set; }
        public string DepartureMonth { get; set; }
        public int DepartureWeek { get; set; }
        public int DepartureDays { get; set; }
        public bool TripUpdatesOnWhatsApp { get; set; }
        public string HotelCategoryRating { get; set; }
        public string DomesticFlights { get; set; }
        //public string Star2 { get; set; }
        //public string Star3 { get; set; }
        //public string Star4 { get; set; }
        public string Accommodations { get; set; }
        public bool Homestays { get; set; }
        public bool VillasCottages { get; set; }
        public bool Resorts { get; set; }
        public bool Hotels { get; set; }
        [RegularExpression("^[0-9]+$", ErrorMessage = "Please enter valid Number")]
        public decimal BudgetWithoutAirfare { get; set; }
       

        public int Adults { get; set; }
        public int Infants { get; set; }
        public int Children { get; set; }
        public string YourAge { get; set; }
       
        public bool CabForLocalSightseeingYes { get; set; }
        public bool CabForLocalSightseeingNo { get; set; }

        public string CabForLocalSightseeing { get; set; }
        public string BookWhen { get; set; }
        public string TypeOfTrip { get; set; }
        public string PreferredTimeToCallYou { get; set; }
        public string RequiredInYourLandPackage { get; set; }
        
      
        public string InputTimeZone { get; set; }
        //public bool YourAge18To25 { get; set; }
        //public bool YourAge26To35 { get; set; }
        //public bool YourAge36To40 { get; set; }
        //public bool YourAge41To50 { get; set; }
 
        public bool CustomizablePackage { get; set; }
        [DisplayName("Special Considerations")]
        public string SpecialConsiderations { get; set; }
        [DisplayName("Arrival And Departure DateTime")]

        public string ArrivalAndDepartureDateTime { get; set; }
        public string HotelChoiceIfAny { get; set; }
        public string Other { get; set; }
        public bool BestsellingStandardPackage { get; set; }
        public List<SelectListItem> DestinationList { get; set; }
        public List<SelectListItem> TimezoneList { get; set; }

        public string SupportEmail { get; set; } = "";
        public string SupportMobile { get; set; } = "";
        public string LogoImageName { get; set; } = "";
        public string LogoImageNameDark { get; set; } = "";
    }

    public class OptionsModel
    {
        public int Id { get; set; }
        public string Option { get; set; }
        public string? OptionDescription { get; set; }
        public string QuesKey { get; set; }
    }
    public class QuestionsModel
    {
        public string QuestionOrOptionName { get; set; }
        public bool IFieldRequired { get; set; }
        public string QueOptionDescription { get; set; }
        public string QuesKey { get; set; }
        public int SectionId { get; set; }
        public List<OptionsModel> Optionslist { get; set; }
    }
}
