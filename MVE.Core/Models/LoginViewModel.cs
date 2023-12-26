using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVE.Core.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        
        [Required(ErrorMessage = "Please enter password")]
       [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*\\W).{8,16}$", ErrorMessage = "Password must contains atleast 1 Uppercase , 1 Lowercase ,1 Numeric Character, 1 Special Character and length should be 8 to 16 Characters")]        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }


        public string SupportEmail { get; set; } = "";
        public string SupportMobile { get; set; } = "";

        public string LogoImageName { get; set; } = "";
        public string LogoImageNameDark { get; set; } = "";

    }
}
