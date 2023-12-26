using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels
{
    public class GeneralSiteSettingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Site name required")]
        public string SiteName { get; set; } = null!;

        public string LogoImageName { get; set; } = null!;
        public string LogoImageNameDark { get; set; } = null!;
        [Required(ErrorMessage = "Support email address required")]
        public string SupportEmail { get; set; } = null!;
        [Required(ErrorMessage = "Support mobile number required")]
        public string SupportMobile { get; set; } = null!;
        [Required(ErrorMessage = "Address required")]
        public string Address { get; set; } = null!;

        public string? ApplyTaxPercentHeading1 { get; set; }

        public long? ApplyTaxPercent1 { get; set; }

        public string? ApplyTaxPercentHeading2 { get; set; }

        public long? ApplyTaxPercent2 { get; set; }
        [Required(ErrorMessage = "Admin page limit required")]
        public int AdminPageLimit { get; set; }

        public bool IsApplyDiscountPercent { get; set; }

        public long? DiscountPercent { get; set; }

        public bool IsApplyDiscountFix { get; set; }

        public long? DiscountFix { get; set; }

        public bool IsActive { get; set; }

        public IFormFile? File { get; set; }
        public IFormFile? FileDark { get; set; }
    }

    public class AdminPersonalDetailsViewModel
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string LastUpdateDate { get; set; }
        public string RoleName { get; set; }
    }
}