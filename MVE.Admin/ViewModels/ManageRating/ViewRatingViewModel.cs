namespace MVE.Admin.ViewModels
{
    public class ViewRatingViewModel
    {
        public long Id { get; set; }
        public string CustomerImage { get; set; }
        public string CustomerName { get; set; }
        public string PackageImage { get; set; }
        public string PackageName { get; set; }
        public string Rating { get; set; }
        public string ReviewDescription { get; set; }
        public string RatingDate { get; set; }
        public string ReviewDate { get; set; }
    }    
    public class RatingResponseDTO
    {
        public long Id { get; set; }

        public string CustomerImage { get; set; }
        public string CustomerName { get; set; }
        public string PackageImage { get; set; }
        public string PackageName { get; set; }
        public string Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
