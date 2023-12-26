using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public long? PermissionId { get; set; }

    public int? OrderIndex { get; set; }

    public string? Url { get; set; }

    public string? Name { get; set; }

    public string? Icon { get; set; }

    public string? SubMenuName { get; set; }

    public string? SubMenuIcon { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long ModifiedBy { get; set; }
}
