using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVE.Admin.ViewModels.StaticContentBanner
{
    public class StaticContentBannerViewModels
    {
        public int Id { get; set; }

        public int ImageType { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "*required")]
        public int ImageFor { get; set; }
        //public string ImagsName { get; set; }
        public string FileName { get; set; }

        public string ImageName { get; set; } = null!;

        public long? ImageSize { get; set; }

        public string? OriginalImageName { get; set; }

        public string ImageExtension { get; set; } = null!;

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public long? ModifiedBy { get; set; }

        public IFormFile? FlagImage { get; set; }
    }
}
