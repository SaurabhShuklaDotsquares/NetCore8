using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class UserNotification
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Descriptions { get; set; } = null!;

    public int NotificationTypeId { get; set; }

    public long? UserId { get; set; }

    public int? UserIdType { get; set; }

    public long? ContactUsId { get; set; }

    public string? ImageName { get; set; }

    public long? ImageSize { get; set; }

    public string? OriginalImageName { get; set; }

    public string ImageExtension { get; set; } = null!;

    public bool IsImageAdded { get; set; }

    public int? SentType { get; set; }

    public bool IsDeleted { get; set; }

    public bool? IsIncludeInNotification { get; set; }

    public bool IsActive { get; set; }

    public bool? IsVisited { get; set; }

    public int? CreatedByType { get; set; }

    public DateTime CreatedDate { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }
}
