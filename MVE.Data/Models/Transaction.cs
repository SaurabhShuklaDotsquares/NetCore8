using System;
using System.Collections.Generic;

namespace MVE.Data.Models;

public partial class Transaction
{
    public long Id { get; set; }

    public long BookingId { get; set; }

    public decimal PaymentAmt { get; set; }

    public DateTime PaymentDateTime { get; set; }

    public int PaymentMethodId { get; set; }

    public int TransactionStatusId { get; set; }

    public string PmtGatewayResponseData { get; set; } = null!;
    public string? TransactionId { get; set; }

    public string? PaymentType { get; set; }

    public string? PaymentToken { get; set; }

    public DateTime CreationOn { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
