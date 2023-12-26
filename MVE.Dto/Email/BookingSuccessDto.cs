using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Dto
{
    public class BookingSuccessDto
    {
        public long BookingId { get; set; }
        public string TransactionId { get; set; }
        public string BookingDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PackageName { get; set; }
        public string FromDate { get; set; }
        public string Todate { get; set; }
        public string PackageId { get; set; }
        public string PackageUrl { get; set; }
    }
}
