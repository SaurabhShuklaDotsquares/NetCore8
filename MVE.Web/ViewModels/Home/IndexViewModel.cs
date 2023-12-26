

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TCP.Web.ViewModels
{
    public class IndexViewModel
    {
        public List<ThemeViewModel> ThemeList { get; set; }
        public List<PackageListViewModel> PackagesList { get; set; }
        public List<EnumsList> BudgetList { get; set; }
        public List<EnumsList> DurationList { get; set; }
        public List<EnumsList> RegionList { get; set; }
        [RegularExpression("^(?!-)\\d+(\\.\\d+)?$", ErrorMessage = "Number should not be negative")]

        public int Duration { get; set; }
        public string Destination { get; set; }
        public string where_To { get; set; }
        public string where_From { get; set; }
        [RegularExpression("^(?!-)\\d+(\\.\\d+)?$", ErrorMessage = "Number should not be negative")]
        public int Budget { get; set; }

        public string ShortUrl { get; set; }
        public string Imagename { get; set; }

        [Display(Name = "Destination")]
        public List<SelectListItem> DestinationList { get; set; }
    }


    public class EnumsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Active { get; set; }
        public string Description { get; set; }

    }
    public class PackageListViewModel
    {
        public long PkgDtlId { get; set; }
        public long PkgImgId { get; set; }

        public long PackageId { get; set; }
        public int? LocationId { get; set; }
        public decimal PackagePrice { get; set; }
        public string PackagePriceFront { get; set; }
        public bool IsCruseIncluded { get; set; }
        public bool IsTransferIncluded { get; set; }

        public int Rating { get; set; }

        public long BookingNo { get; set; }

        //public string ActivityDate { get; set; }

        //public string? ActivityTime { get; set; }

        //public string ActivityName { get; set; }

        public string? LocationAddress { get; set; }
        public string? PkgDesc { get; set; }
        public string? PackageNoOf_DaysNight { get; set; }
        public int? PackageNoOfDays { get; set; }
        public int RegionId { get; set; }

        public bool IsHotelIncluded { get; set; }

        public bool IsTransportIncluded { get; set; }

        public bool IsMealIncluded { get; set; }

        //public bool IsLunch { get; set; }

        //public bool IsBreakfast { get; set; }

        //public bool IsDinner { get; set; }


        //  public DateTime CreatedOn { get; set; }
        public string PackageName { get; set; }
        public string ImageName { get; set; }
        public string FileOriginalName { get; set; }
        public string FileExtension { get; set; }
        public string FromDate { get; set; }
        public DateTime FromDatetime { get; set; }

        public string FilePath { get; set; }
        public string PackageUrl { get; set; }

    }
}
