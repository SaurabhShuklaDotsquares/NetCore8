using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MVE.Data.Models;

namespace MVE.Core.Models
{
    public class RegisterViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]
        [DisplayName("New Password")]   
        [Required(ErrorMessage = "Please enter Password")]
        public string Password { get; set; }

        [MaxLength(16)]
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please re-enter Password")]
        [Compare("Password", ErrorMessage = "Your new password and confirm password values doesn't match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please Agree with T&C and Privacy Policy")]
        public bool Terms_Conditions { get; set; }



        public string EncryptedPassword { get; set; } = null!;

        public string SaltKey { get; set; } = null!;
        [Required(ErrorMessage = "Please enter Full Name")]
        [MaxLength(40)]
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;


        public DateTime? DateOfBirth { get; set; }

        public byte? Gender { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? StateOrCounty { get; set; }

        public int? CountryId { get; set; }

        public string? ZipCode { get; set; }

        [RegularExpression("^(\\+)?[0-9]{10,16}$", ErrorMessage = "Enter valid Phone Number and length should be 10 to 16 characters")]
        public string? MobilePhone { get; set; }

        public string? ForgotPasswordLink { get; set; }

        public DateTime? ForgotPasswordLinkExpired { get; set; }

        public bool? ForgotPasswordLinkUsed { get; set; }

        public string ImageName { get; set; } = null!;

        public bool IsEmailVerified { get; set; }
        [Required(ErrorMessage = "Please enter OTP")]
        [MaxLength(4)]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Please enter valid OTP")]
        public string? EmailOtp { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsActive { get; set; }

        public string SupportEmail { get; set; } = "";
        public string SupportMobile { get; set; } = "";
        public string LogoImageName { get; set; } = "";
        public string LogoImageNameDark { get; set; } = "";
    }

   
}
