using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class AdminUser
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public string Email { get; set; } = null!;

    public string EncryptedPassword { get; set; } = null!;

    public string SaltKey { get; set; } = null!;

    public byte Title { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public byte Gender { get; set; }

    public string? Address { get; set; }

    public string? MobilePhone { get; set; }

    public string? ForgotPasswordLink { get; set; }

    public DateTime? ForgotPasswordLinkExpired { get; set; }

    public bool ForgotPasswordLinkUsed { get; set; }

    public string? ImageName { get; set; }

    public bool IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreationOn { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual AdminUser? CreatedByNavigation { get; set; }

    public virtual ICollection<AdminUser> InverseCreatedByNavigation { get; set; } = new List<AdminUser>();

    public virtual ICollection<AdminUser> InverseModifiedByNavigation { get; set; } = new List<AdminUser>();

    public virtual AdminUser? ModifiedByNavigation { get; set; }

    public virtual UserRole Role { get; set; } = null!;
}
