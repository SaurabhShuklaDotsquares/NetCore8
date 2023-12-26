using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Theme
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string ShortName { get; set; } = null!;

    public string? Description { get; set; }

    public string ImageName { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

}
