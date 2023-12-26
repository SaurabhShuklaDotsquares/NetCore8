using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace MVE.Admin.ViewModels
{
    public class CountryViewModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "*required")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        public string FileName { get; set; }
        [MaxLength(5,ErrorMessage = "length should be less than or equal to 5")]
        public string Code { get; set; }

        [DisplayName("Status")]
        public bool IsActive { get; set; }

             
        //[Required(ErrorMessage = "Please select files")]
        public IFormFile? FlagImage { get; set; }
      
    }
}
