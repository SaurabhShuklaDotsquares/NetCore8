using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class GeneralSiteSetting
{
    public int Id { get; set; }

    public string KeyName { get; set; } = null!;

    public string SiteName { get; set; } = null!;

    public string LogoImageName { get; set; } = null!;

    public long? LogoImageSize { get; set; }

    public string? LogoOriginalImageName { get; set; }

    public string LogoImageExtension { get; set; } = null!;

    public string LogoImageNameDark { get; set; } = null!;

    public long? LogoImageSizeDark { get; set; }

    public string? LogoOriginalImageNameDark { get; set; }

    public string LogoImageExtensionDark { get; set; } = null!;

    public string SupportEmail { get; set; } = null!;

    public string SupportMobile { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? ApplyTaxPercentHeading1 { get; set; }

    public decimal? ApplyTaxPercent1 { get; set; }

    public string? ApplyTaxPercentHeading2 { get; set; }

    public decimal? ApplyTaxPercent2 { get; set; }

    public int AdminPageLimit { get; set; }

    public bool? IsApplyDiscountPercent { get; set; }

    public decimal? DiscountPercent { get; set; }

    public bool? IsApplyDiscountFix { get; set; }

    public decimal? DiscountFix { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public long? ModifiedBy { get; set; }
}
