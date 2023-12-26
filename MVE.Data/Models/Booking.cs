using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Booking
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PackageId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? CountryCode { get; set; }

    public string MobilePhone { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public string? PassportNo { get; set; }

    public int AdultsCount { get; set; }

    public int ChildsCount { get; set; }

    public int? InfantsCount { get; set; }

    public string? ApplyTaxPercentHeading1 { get; set; }

    public decimal? ApplyTaxPercent1 { get; set; }

    public string? ApplyTaxPercentHeading2 { get; set; }

    public decimal? ApplyTaxPercent2 { get; set; }

    public decimal? AppliedDiscountPercent { get; set; }

    public decimal? AppliedFixedDiscunt { get; set; }

    public string? SpecialRequests { get; set; }

    public int CountryId { get; set; }

    public int StateId { get; set; }

    public string City { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int BookingStatusId { get; set; }

    public string? BookingFor { get; set; }

    public DateTime CreationOn { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User User { get; set; } = null!;
}
