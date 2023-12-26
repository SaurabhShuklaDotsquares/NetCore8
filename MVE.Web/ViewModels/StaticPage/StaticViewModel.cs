using System.ComponentModel;

namespace TCP.Web.ViewModels
{
    public class StaticViewModel
    {
        //public Guid Id { get; set; }
        public int StaticPageId { get; set; }
        public string Name { get; set; }
        [DisplayName("Page Title")]
        public string PageTitle { get; set; }
        public string Content { get; set; }
        [DisplayName("Meta Keyword")]
        public string MetaKeyword { get; set; }
        [DisplayName("Meta Description")]
        public string MetaDescription { get; set; }
        [DisplayName("Url")]
        public string SelfUrl { get; set; }
        public string Url { get; internal set; }
    }
}
