using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels
{
    public class AccomodationViewModel
    {
        public int Id { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "*required")]
        public string Name { get; set; }
        public string Description { get; set; }

        [DisplayName("Status")]
        public bool IsActive { get; set; }
       
    }
}
