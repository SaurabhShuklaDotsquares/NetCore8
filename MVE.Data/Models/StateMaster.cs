using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class StateMaster
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public int? StateCode { get; set; }

    public string? Code { get; set; }

    public DateTime CreationOn { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual CountryMaster IdNavigation { get; set; } = null!;
}
