using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TCP.Web.ViewModels
{
    public class ContactUsViewModel
    {
        [Required(ErrorMessage = "Please enter full name")]
        [MaxLength(40)]
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Please enter subject")]

        public string Subject { get; set; }

        [Required(ErrorMessage = "Please enter phone number")]
        [RegularExpression("^(\\+)?[0-9]{10,16}$", ErrorMessage = "Enter valid phone number and length should be 10 to 16 characters")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter message")]

        public string Message { get; set; } = "";
        public string SupportEmail { get; set; } = "";
        public string SupportMobile { get; set; } = "";

        public string LogoImageName { get; set; } = "";
        public string LogoImageNameDark { get; set; } = "";
    }
}
