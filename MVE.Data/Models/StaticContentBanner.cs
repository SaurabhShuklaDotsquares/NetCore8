using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class StaticContentBanner
{
    public int Id { get; set; }

    public int ImageType { get; set; }

    public int ImageFor { get; set; }

    public string ImageName { get; set; } = null!;

    public long? ImageSize { get; set; }

    public string? OriginalImageName { get; set; }

    public string ImageExtension { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }
}
