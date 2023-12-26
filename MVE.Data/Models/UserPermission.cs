using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class UserPermission
{
    public long Id { get; set; }

    public int UserPermissionId { get; set; }

    public long RoleId { get; set; }

    public DateTime CreatedOn { get; set; }

    public long? CreatedBy { get; set; }

    public virtual UserRole Role { get; set; } = null!;
}
