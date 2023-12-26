using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCP.Data.Models;

namespace TCP.Web.ViewModels
{
    public class DestinationGuideViewModel
    {
        public long DestHeadId { get; set; }
        public long DestMidFootId { get; set; }

        [Required(ErrorMessage = "The Heading Title field is required")]
        public string HeadingTitle { get; set; }
        [Required(ErrorMessage = "The Heading Content(Description) field is required")]

        public string HeadingContent { get; set; }
        public string CountryName { get; set; }
        public List<string> lstImageNames { get; set; }
        [Required(ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<IFormFile>? DestImages { get; set; }
        public List<CountryViewModel> CountryMasterslst { get; set; } // for destination guide menu work listing

        //<---------- Middle & Footer Destination Guide----------->
        [Required(ErrorMessage = "The Middle Title field is required")]

        public string MiddleTitle { get; set; }
        [Required(ErrorMessage = "The Middle Content(Description) field is required")]

        public string MiddleContent { get; set; }
        public string MiddleImageNameMain { get; set; }
        public string MiddleOriginalImageNameMain { get; set; }
        public string MiddleImageExtensionMain { get; set; }
        public string MiddleImageName1 { get; set; }
        
        public string MiddleImageName2 { get; set; }
        
        public string MiddleImageName3 { get; set; }
        
        public string MiddleImageName4 { get; set; }
        
        [Required(ErrorMessage = "The Footer Title field is required")]

        public string FooterTitle { get; set; }
        [Required(ErrorMessage = "The Footer Content(Description) Title field is required")]

        public string FooterContent { get; set; }
        public string FooterImageName1 { get; set; }
        
        public string FooterImageExtension1 { get; set; }
        public string FooterImageName2 { get; set; }
        public string FooterOriginalImageName2 { get; set; }
        public string FooterBottomContent { get; set; }
        public IFormFile? MidImage { get; set; }
        public IFormFile? MidImage1 { get; set; }
        public IFormFile? MidImage2 { get; set; }
        public IFormFile? MidImage3 { get; set; }
        public IFormFile? MidImage4 { get; set; }


        public IFormFile? FooterImage1 { get; set; }
        public IFormFile? FooterImage2 { get; set; }

    }

}
