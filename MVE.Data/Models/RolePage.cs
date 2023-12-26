using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class RolePage
{
    public long Id { get; set; }

    public string PageName { get; set; } = null!;

    public string? PageUrl { get; set; }

    public int OrderIndex { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<RolePagePermission> RolePagePermissions { get; set; } = new List<RolePagePermission>();
}
