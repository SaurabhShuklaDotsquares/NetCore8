using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TCP.Data.Models;
using TCP.Dto;

namespace TCP.Web.ViewModels
{
    public class TravellersViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid")]
        
        public string EmailAddress { get; set; }

        [MaxLength(40)]
        [Required(ErrorMessage = "Please enter Full Name")]
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter Address")]

        public string Address { get; set; }
        public string? PassportNo { get; set; }
        public string? SpecialRequests { get; set; }
        [Required(ErrorMessage = "Please enter Town/City")]

        public string City { get; set; }

        [Required(ErrorMessage = "Please select Country")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Please select State based on Country")]
        public int StateId { get; set; }
        public long PackageId { get; set; }
     
        [Required(ErrorMessage = "Please enter Zip Code")]
        public string? ZipCode { get; set; }
        [MaxLength(16)]
        [Required(ErrorMessage = "Please enter Phone Number")]
        //[RegularExpression("^(?!0{10})\\d{10}$", ErrorMessage = "Phone No. is not valid")]
        [RegularExpression("^(\\+)?[0-9]{10,16}$", ErrorMessage = "Enter valid Phone Number and length should be 10 to 16 characters")]
        public string MobilePhone { get; set; }
        public string CountryCode { get; set; }
        public string BookingFor { get; set; }
   
        public List<SelectListItem> CountyList { get; set; }
        public List<SelectListItem> StateList { get; set; }


        [Required(ErrorMessage = "Please check the Acknowledgement")]
        public bool IsAcknowlwdge { get; set; }


    }
    public class PackageDetailsViewModel
    {
        public long Id { get; set; }
        public long PkgDtlId { get; set; }
        public long PkgImgId { get; set; }

        public long PackageId { get; set; }
        public int? LocationId { get; set; }
        public decimal PackagePrice { get; set; }
        public string PackagePriceFront { get; set; }
        public bool IsCruseIncluded { get; set; }
        public bool IsTransferIncluded { get; set; }

        public int Rating { get; set; }

        public int UserRating { get; set; }
        public string? LocationAddress { get; set; }
        public string? PkgDesc { get; set; }
        public string? Description { get; set; }
        public string? PackageNoOf_DaysNight { get; set; }
        public int? PackageNoOfDays { get; set; }
        public int? PackageNoOfNights { get; set; }

        public bool IsHotelIncluded { get; set; }

        public bool IsTransportIncluded { get; set; }

        public bool IsMealIncluded { get; set; }
        public string PackageName { get; set; }
        public string ImageName { get; set; }
        public string FileOriginalName { get; set; }
        public string FileExtension { get; set; }
        public string Review { get; set; }

        public string FilePath { get; set; }
        public List<string> FilePathList { get; set; }

        public string CountryName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<PackageDetailDTO> PackageDetails { get; set; }
        public List<PackageInclusion> PackageInclusions { get; set; }
        public List<PackageExclusion> PackageExclusions { get; set; }
        public List<PackageHighlight> PackageHighlights { get; set; }

        public List<ReviewViewModel> ReviewViewModel { get; set; }
        //public List<PackageImage> PackageImages { get; set; }
        public List<PackageDetailsViewModel> SimilarPackages { get; set; }
        [Display(Name = "Destination")]
        public List<SelectListItem> DestinationList { get; set; }
        public string where_To { get; set; }
        public string where_From { get; set; }
        public string PackageUrl { get; set; }
    }

    public class ReviewViewModel
    {

        public int PackageId { get; set; }

        public long UserId { get; set; }
        public int UserRating { get; set; }
        public string? ReviewText { get; set; }
        public string? PkgDesc { get; set; }
        public string? Description { get; set; }

        public string? Username { get; set; }

        public string UserImage { get; set; }

        //public string ImageName { get; set; }



    }

}
