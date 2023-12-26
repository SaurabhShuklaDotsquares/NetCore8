using System.ComponentModel.DataAnnotations;
using MVE.Data.Models;

namespace MVE.Admin.ViewModels
{
    public class VisaGuideViewModels
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        [Required(ErrorMessage = "The Content(Description) field is required")]
        public string? ContentData { get; set; }
        [Required(ErrorMessage = "The PageTitle field is required")]
        public string? PageTitle { get; set; }
        public string? MetaKeyword { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string? Ipaddress { get; set; }
        public int? PageSequence { get; set; }
    }
}
