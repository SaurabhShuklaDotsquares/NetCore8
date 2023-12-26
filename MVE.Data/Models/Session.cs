using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Session
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTime SessionExpiry { get; set; }

    public string? SessionData { get; set; }

    public DateTime CreationOn { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual User User { get; set; } = null!;
}
