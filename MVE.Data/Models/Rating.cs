using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Rating
{
    public int Id { get; set; }

    public long PackageId { get; set; }

    public long UserId { get; set; }

    public int RatingVal { get; set; }

    public int? UserType { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime RatingOn { get; set; }
    public virtual User User { get; set; } = null!;
}
