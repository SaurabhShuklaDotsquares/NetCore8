namespace TCP.Web.ViewModels.VisaGuide
{
    public class VisaGuideViewModel
    {
        public int Id { get; set; }
        public int? CountryId { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string? ContentData { get; set; }
        public string? PageTitle { get; set; }
        public string CountryName { get; set; }
        public string? MetaKeyword { get; set; }
        public string? MetaDescription { get; set; }
        public bool IsActive { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string? Ipaddress { get; set; }
        public int? PageSequence { get; set; }
        public List<CountryViewModel> CountryMasterslst { get; set; }
    }
}
