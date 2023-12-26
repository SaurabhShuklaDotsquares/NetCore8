using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVE.Admin.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Old password required")]
        [Remote("IsPasswordExist", "AdminUser", AdditionalFields = "UserId", ErrorMessage = "Incorrect old password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password required")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]
        public string NewPassword { get; set; }

        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Confirm password required")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again")]
        public string ConfirmPassword { get; set; }

    }
}
