using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MVE.Admin.ViewModels.Notifications
{
    public class ManageNotificationViewModel
    {
        public int Id { get; set; }
        [DisplayName("Title")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description should be less than 500 characters")]
        public string Description { get; set; }

        public string? FileName { get; set; }
        //[MaxLength(5, ErrorMessage = "length should be less than or equal to 5")]

        [Required(ErrorMessage = "File is required.")]
        public IFormFile? FlagImage { get; set; }
        public string[]? SelectedUsers { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? MobilePhone { get; set; }
    }
}
