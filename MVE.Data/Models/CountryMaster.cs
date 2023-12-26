using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class CountryMaster
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? Code { get; set; }

    public string? Description { get; set; }

    public string Image { get; set; } = null!;

    public string Icon { get; set; } = null!;

    public long CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public string? ShortUrl { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
    public virtual StateMaster? StateMaster { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<VisaGuide> VisaGuides { get; set; } = new List<VisaGuide>();
}
