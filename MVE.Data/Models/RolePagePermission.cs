using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class RolePagePermission
{
    public long Id { get; set; }

    public long PageId { get; set; }

    public long RoleId { get; set; }

    public bool IsReadOnly { get; set; }

    public bool IsCreate { get; set; }

    public bool IsEdit { get; set; }

    public bool IsDelete { get; set; }

    public virtual RolePage Page { get; set; } = null!;

    public virtual UserRole Role { get; set; } = null!;
}
