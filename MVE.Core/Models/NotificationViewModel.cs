using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MVE.Data.Models;

namespace MVE.Core.Models
{
    public class NotificationViewModel
    {
        public long Id { get; set; }
        public long ContactUsId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactUser { get; set; }
        [Required(ErrorMessage = "Please select user")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please enter subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Please enter query")]
        public string Query { get; set; }             

        public List<SelectListItem> UsersList { get; set; }
        public string ImageName { get; set; } = null!;

        public IFormFile? QueryImage { get; set; }

        public bool IsDeleted { get; set; }

        public bool? IsActive { get; set; }
        public string TicketId { get; set; }

        public long ReplyToUserId { get; set; }


    }

   
}
