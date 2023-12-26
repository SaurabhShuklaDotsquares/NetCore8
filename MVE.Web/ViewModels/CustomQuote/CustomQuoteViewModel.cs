using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TCP.Core.Models.Security;

namespace TCP.Web.ViewModels
{
    public class CustomQuoteViewModel
    {
        [DisplayName("Pre-built Package")]
        [Required(ErrorMessage = "Please enter Pre-built Package Url")]
        public string packageUrl { get; set; }
        [Required(ErrorMessage = "Please enter the Requirement ")]
        public string Requirement { get; set; }

        [Required(ErrorMessage = "Please enter Email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter Phone Number")]
        [MaxLength(16)]
        [RegularExpression("^(\\+)?[0-9]{10,16}$", ErrorMessage = "Enter valid Phone Number and length should be 10 to 16 characters")]
        //[Range(10, 16, ErrorMessage = " Phone Number length should be 10 to 16 character")]
        public string PhoneNumber { get; set; }
    public long? CustomizedPkgId { get; set; }
    public long? CustomizeRequestParentId { get; set; }
}
}
