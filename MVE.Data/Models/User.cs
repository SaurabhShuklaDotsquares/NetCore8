using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string EncryptedPassword { get; set; } = null!;

    public string SaltKey { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public byte? Gender { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? StateOrCounty { get; set; }

    public int? CountryId { get; set; }

    public string? ZipCode { get; set; }

    public string? MobilePhone { get; set; }

    public string? ForgotPasswordLink { get; set; }

    public DateTime? ForgotPasswordLinkExpired { get; set; }

    public bool? ForgotPasswordLinkUsed { get; set; }

    public string ImageName { get; set; } = null!;

    public bool IsEmailVerified { get; set; }

    public string? EmailOtp { get; set; }

    public DateTime? EmailVerificationOtpExpired { get; set; }

    public string? EmailVerificationToken { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreationOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public long? ModifiedBy { get; set; }

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CountryMaster? Country { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
