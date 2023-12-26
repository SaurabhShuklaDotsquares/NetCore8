using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace MVE.Admin.ViewModels
{
    public class StaticViewModel
    {
        //public Guid Id { get; set; }
        public int StaticPageId { get; set; }
        public string Name { get; set; }
        [DisplayName("Page Title")]
        [Required(ErrorMessage = "Page title is required")]
        public string PageTitle { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        [DisplayName("Meta Keyword")]
        [Required(ErrorMessage = "Meta keyword is required")]
        public string MetaKeyword { get; set; }
        [DisplayName("Meta Description")]
        [Required(ErrorMessage = "Meta description is required")]
        public string MetaDescription { get; set; }
        [DisplayName("Url")]
        public string SelfUrl { get; set; }
    }
}
