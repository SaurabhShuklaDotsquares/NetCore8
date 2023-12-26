

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TCP.Web.ViewModels
{
    public class ListingViewModel
    {

        public List<PackageListViewModel> PackagesList { get; set; }
        public List<EnumsList> HotelRatings { get; set; }
        public List<EnumsList> PackageInclusionslst { get; set; }
        public List<SelectListItem> PackageDirectionlst { get; set; }
        public List<EnumsList> BudgetList { get; set; }
        public List<EnumsList> DurationList { get; set; }
        public List<SelectListItem> CountryList { get; set; }

        public int Duration { get; set; }
        public string Destination { get; set; }
        public int Budget { get; set; }
        public int totalItem { get; set; }
        public int CurrentPageIndex { get; set; }

        //public long PkgDtlId { get; set; }
        //public long PkgImgId { get; set; }

        public long PackageId { get; set; }
        //public int? LocationId { get; set; }
        //public decimal PackagePrice { get; set; }
        //public bool IsCruseIncluded { get; set; }       

        //public string? LocationAddress { get; set; }
        //public string? PkgDesc { get; set; }
        //public string? PackageNoOf_DaysNight { get; set; }


        public string RegionName { get; set; }
        //public string ImageName { get; set; }
        //public string FileOriginalName { get; set; }
        //public string FileExtension { get; set; }

        //public string FilePath { get; set; }

        public string ThemeName { get; set; }
        public string CountryName { get; set; }
        public string BestPlace { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string MaxPriceFront { get; set; }
    }
}
