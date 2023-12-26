using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Code
{
    public enum UploadSection : int
    {
        [Display(Name = "Lead Image", GroupName = UploadSectionGroup.PackageImage, Prompt = FileUploaderType.Single)]
        PackageLead = 1,
        //[Display(Name = "Gallery", GroupName = UploadSectionGroup.PackageImage, Prompt = FileUploaderType.Multiple, ShortName = "Titles")]
        [Display(Name = "Gallery", GroupName = UploadSectionGroup.PackageImage, Prompt = FileUploaderType.Multiple)]
        PackageGallery = 2,

        [Display(Name = "Image", GroupName = UploadSectionGroup.PackageImage, Prompt = FileUploaderType.Single)]
        PackageDetailActivity = 3,
        [Display(Name = "Image", GroupName = UploadSectionGroup.HotelImages, Prompt = FileUploaderType.Multiple)]
        HotelImage = 4,

        [Display(Name = "GalleryVideos", GroupName = UploadSectionGroup.PackageVideo, Prompt = FileUploaderType.Multiple)]
        PackageGalleryVideos = 5,
    }
}
