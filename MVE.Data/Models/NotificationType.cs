using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class NotificationType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public bool? IsActive { get; set; }
}
