using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCP.Web.ViewModels
{
    public class MyProfileViewModel
    {       
        public PersonalViewModel PersonalObj { get; set; }
        public BillingViewModel BillingObj { get; set; }
    }

    public class PersonalViewModel
    {
        public long UserId { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]

        public string EmailAddress { get; set; }

        
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter first name")]

        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Please enter last name")]

        public string LastName { get; set; } = null!;


        public string? Address { get; set; }
       
        [MaxLength(16)]
        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression("^(\\+)?[0-9]{10,16}$", ErrorMessage = "Enter valid phone number and length should be 10 to 16 characters")]
        public string MobilePhone { get; set; }
        public string? DateofBirth { get; set; }
        
    }
    public class BillingViewModel
    {
        [Required(ErrorMessage = "Please enter town/city")]

        public string TownorCity { get; set; } 
        [Required(ErrorMessage = "Please enter address1")]

        public string Address1 { get; set; }
        [Required(ErrorMessage = "Please enter address2")]

        public string Address2 { get; set; }

        [Required(ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        public long BillingId { get; set; }
        [Required(ErrorMessage = "Please select state based on country")]
        public int StateId { get; set; }

        public string? ZipCode { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public List<SelectListItem> CountryList { get; set; }
    }
}
