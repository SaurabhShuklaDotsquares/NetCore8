using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TCP.Web.ViewModels
{
    public class RatingReviewViewModel
    {
        public long RatingId{ get; set; }
        public long PackageId { get; set; }
        public long BookingId { get; set; }
        public long ReviewId { get; set; }
        [Required(ErrorMessage = "Please select any rating")]

        public int Rating { get; set; }
       // [Required(ErrorMessage = "Please enter some review description")]

        public string? ReviewText { get; set; }
        public string PackageName { get; set; }
    }
}
