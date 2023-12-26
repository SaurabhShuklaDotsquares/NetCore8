using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class VisaGuide
{
    public int Id { get; set; }

    public int CountryId { get; set; }

    public string Name { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string ContentData { get; set; } = null!;

    public string? PageTitle { get; set; }

    public string? MetaKeyword { get; set; }

    public string? MetaDescription { get; set; }

    public bool? IsActive { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual CountryMaster Country { get; set; } = null!;
}
