using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "*required")]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public long CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public List<SelectListItem> ParentCategoryList { get; set; } = new List<SelectListItem>();
        public int? SubCatCount { get; set; }
    }
}
