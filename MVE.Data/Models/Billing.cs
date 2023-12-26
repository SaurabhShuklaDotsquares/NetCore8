using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Billing
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public string? Town { get; set; }

    public int? StateId { get; set; }

    public int? Country { get; set; }

    public string? Zipcode { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual CountryMaster? CountryNavigation { get; set; }

    public virtual StateMaster? State { get; set; }

    public virtual User User { get; set; } = null!;
}
