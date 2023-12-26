namespace TCP.Web.ViewModels
{
    public class PaymentSuccessViewModel
    {
        public long BookingId { get; set; }
        public string TransactionId { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }
    }
}
