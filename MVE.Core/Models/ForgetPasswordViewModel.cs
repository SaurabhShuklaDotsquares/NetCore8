using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Models
{
    public class ForgetPasswordViewModel
    {

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }
        public string? ForgetPasswordLink { get; set; }
        public string? ForgetPasswordMessage { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public int? Id { get; set; }

        public string? FirstName { get; set; }

        public string? replyName { get; set; }


        public string SupportEmail { get; set; } = "";
        public string SupportMobile { get; set; } = "";
        public string LogoImageName { get; set; } = "";
        public string LogoImageNameDark { get; set; } = "";

    }
}
