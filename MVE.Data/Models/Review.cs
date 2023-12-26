using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Review
{
    public long ReviewId { get; set; }

    public long PackageId { get; set; }

    public long UserId { get; set; }

    public string ReviewText { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime ReviewOn { get; set; }

    public virtual User User { get; set; } = null!;
}
