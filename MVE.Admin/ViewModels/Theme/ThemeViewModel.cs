using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels
{
    public class ThemeViewModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "Name id required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Name id required")]
        public string ShortName { get; set; }
        public string Description { get; set; }

        [DisplayName("Status")]
        public bool IsActive { get; set; }

       // [MaxLength(5, ErrorMessage = "length should be less than or equal to 5")]
        [Required(ErrorMessage = "Please select files")]

       
        public IFormFile? File{ get; set; }
        public string ImageName { get; internal set; }
    }
}
