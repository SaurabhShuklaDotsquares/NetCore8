using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class ActivityIncExcMaster
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Type { get; set; }

    public bool IsDeleted { get; set; }

    public string? Description { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

}
