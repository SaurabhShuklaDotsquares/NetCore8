using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Models
{
    public class ResetPasswordViewModel
    {
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]
        [DisplayName("New Password")]
        [Required(ErrorMessage = "*required")]
        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        [Required(ErrorMessage = "Please re-enter Password")]
        [Compare("Password", ErrorMessage = "Your new password and confirm password values doesn't match")]
        public string ConfirmPassword { get; set; }
        public string Id { get; set; }
    }
}
