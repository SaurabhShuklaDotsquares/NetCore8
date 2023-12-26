using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using TCP.Core.Code.LIBS;
using TCP.Core.Code;
using TCP.Core.Models;
using TCP.Core.Models.Others;
using TCP.Data.Models;
using TCP.Service;
using TCP.Web.ViewModels;
using TCP.Dto;
using TCP.Core.Code.Extensions;
using TCP.Service.ActivityIncExc;
using System.IO;

namespace TCP.Web.Controllers
{
    public class ListingDetailController : BaseController
    {
        private readonly ICountryService _countryService;
        private readonly IEmailFactoryService _emailFactoryService;
        private readonly IUserService _userService;
        private readonly TravelCustomPackagesContext _context;
        private readonly IPackageService _pkgservice;
        private readonly IActivityIncExcService _activityIncExcService;

        public ListingDetailController(ICountryService countryService, IUserService userService, IEmailFactoryService emailFactoryService, TravelCustomPackagesContext context, IPackageService pkgservice, IActivityIncExcService activityIncExcService)
        {
            _countryService = countryService;
            _userService = userService;
            _emailFactoryService = emailFactoryService;
            _context = context;
            _pkgservice = pkgservice;
            _activityIncExcService = activityIncExcService;
        }

        public IActionResult Index(string package_url)
        {
            long? pkgid = 0;
            //Developer
            PackageDetailsViewModel vm = new PackageDetailsViewModel();
            //var Ids = pkgid ?? 0;
            vm.Id = 0;
            Package packages1 = _pkgservice.GetActivePackageByUrl(package_url);
            Package packages = _pkgservice.GetActivePackageById(packages1.Id);

            if (packages != null)
            {
                vm.Id = packages.Id;
                vm = BindPackageDetailsData(packages, packages.Id);
            }

            var ratings = _pkgservice.GetRatingListById(Convert.ToInt64(vm.Id));
            var reviews = _pkgservice.GetReviewListById(Convert.ToInt64(vm.Id));

            vm.ReviewViewModel = new List<ReviewViewModel>();

            string path_userprofile = Path.Combine(SiteKeys.Domain + "/" + SiteKeys.UploadFilesUsers);
            vm.ReviewViewModel = ratings.Select(x => new ReviewViewModel
            {
                PackageId = (int)x.PackageId,
                UserRating = x.RatingVal,
                ReviewText = reviews.Where(r => r.PackageId == x.PackageId && r.UserId == x.UserId).FirstOrDefault()?.ReviewText,
                UserId = x.UserId
                //Username =  x.User.FirstName.Where(r.UserId == x.UserId). 
                //UserImage = !string.IsNullOrEmpty(_userService.GetUserById(Convert.ToInt64(x.UserId)).ImageName) ? Path.Combine(path_userprofile, x.User.ImageName) : SiteKeys.Domain + "/images/demo_user.jpg"

            }).ToList();

            foreach (var key in vm.ReviewViewModel)
            {
                var _user = _userService.GetUserById(Convert.ToInt64(key.UserId));
                key.UserImage = SiteKeys.ImageDomain + "/images/demo_user.jpg";
                if (_user != null)
                {
                    if (!string.IsNullOrEmpty(_user.ImageName))
                    {
                        key.UserImage = Path.Combine(path_userprofile, _user.ImageName);
                    }
                    key.Username = _user.FirstName + " " + _user.LastName;
                }
            }



            vm.DestinationList = _countryService.GetCountryMastersForDropDownWithShortURL().OrderBy(o => o.Text).ToList();
            return View(vm);
        }
        private PackageDetailsViewModel BindPackageDetailsData(Package item, long pkgids)
        {
            PackageDetailsViewModel pkgObj = new PackageDetailsViewModel();
            pkgObj.Id = pkgids;
            pkgObj.PackageId = item.Id;
            pkgObj.PackageUrl = item.PackageUrl;
            //pkgObj.Review = item.Reviews!=null&&item.Reviews.Count() > 0 ? item.Reviews.FirstOrDefault().ReviewText : "Static Review";
            //pkgObj.UserRating = item.Ratings!=null&&item.Ratings.Count() > 0 ? item.Ratings.FirstOrDefault().RatingVal : 0;
            //pkgObj.Review = item.Reviews == null || item.Reviews?.Count() == 0 ? "Static Review": item.Reviews.FirstOrDefault().ReviewText;
            //pkgObj.UserRating = item.Ratings == null || item.Ratings.Count() == 0 ? 0:item.Ratings.FirstOrDefault().RatingVal;
            pkgObj.LocationAddress = item.Country?.Name ?? "-";
            pkgObj.LocationId = item.Country?.Id;
            pkgObj.PkgDtlId = item.Id;
            pkgObj.IsCruseIncluded = item.IsCruseIncluded;
            pkgObj.IsTransferIncluded = item.IsTransferIncluded;
            pkgObj.IsTransportIncluded = item.IsTransportIncluded;
            pkgObj.IsHotelIncluded = item.IsHotelIncluded;
            pkgObj.IsMealIncluded = item.IsMealIncluded;
            pkgObj.PackagePrice = item.PackagePrice;
            pkgObj.PackagePriceFront = item.PackagePrice.ToString("0.##");
            pkgObj.PackageName = item.Name;
            pkgObj.PkgDesc = item.Description?.Length <= 83 ? item.Description : item.Description?.Substring(0, 83) + "..."; ;
            pkgObj.Description = item.Description;
            pkgObj.PackageNoOf_DaysNight = item.PackageNoOfDays.ToString() + " Days & " + item.PackageNoOfNights.ToString() + " Nights";
            pkgObj.PackageNoOfDays = item.PackageNoOfDays;
            pkgObj.PackageNoOfNights = item.PackageNoOfNights;

            pkgObj.FilePathList = new List<string>();
            var imagesObjlst = item.PackageImages;
            if (imagesObjlst != null)
            {
                foreach (var imagesObj in imagesObjlst)
                {
                    if (imagesObj != null)
                    {
                        pkgObj.FileOriginalName = imagesObj.OriginalImageName ?? "";
                        pkgObj.FileExtension = imagesObj.ImageExtension;
                        pkgObj.PkgImgId = imagesObj.Id;
                        //pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin((UploadSection)imagesObj.ImageSection, imagesObj.ImageName, imagesObj.ImageExtension, imagesObj.OriginalImageName);
                        string RetImageName = "";
                        pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);

                        var FilePathUrl = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);
                        pkgObj.FilePathList.Add(FilePathUrl);
                        imagesObj.ImageName = RetImageName;
                    }
                }
            }
            pkgObj.CountryName = item.Country?.Name ?? "";
            pkgObj.FromDate = item.FromDate.ToString("dd MMM, yyyy");
            pkgObj.ToDate = item.ToDate.ToString("dd MMM, yyyy");
            //pkgObj.PackageDetails = BindPackageDetailsListData(item.PackageDetails.ToList());
            pkgObj.PackageDetails = BindPackageDetailsListData(item.PackageDetails.ToList());


            pkgObj.PackageInclusions = item.PackageInclusions.ToList();
            pkgObj.PackageExclusions = item.PackageExclusions.ToList();
            pkgObj.PackageHighlights = item.PackageHighlights.ToList();

            pkgObj.SimilarPackages = new List<PackageDetailsViewModel>();

            var _SimilarPackages = GetSimilarPackages(item.ThemeId);
            if (_SimilarPackages != null && _SimilarPackages.Count > 0)
            {
                pkgObj.SimilarPackages = _SimilarPackages.Where(x => x.PackageId != item.Id).ToList();
            }




            var pkgDtls = item.PackageDetails?.FirstOrDefault();
            if (pkgDtls != null)
            {
                var hoteldtls = pkgDtls.PackageHotels.FirstOrDefault();
                if (hoteldtls != null)
                {
                    pkgObj.Rating = hoteldtls.Hotel?.Rating ?? 0;
                }
            }
            return pkgObj;
        }

        public List<PackageDetailsViewModel> GetSimilarPackages(int? ThemeId)
        {
            List<PackageDetailsViewModel> modelsLst = new List<PackageDetailsViewModel>();
            var packages = _pkgservice.GetPackagesByCategoryList(ThemeId);
            if (packages != null)
            {
                foreach (var item in packages)
                {
                    PackageDetailsViewModel pkgObj = new PackageDetailsViewModel();
                    pkgObj.Id = item.Id;
                    pkgObj.PackageId = item.Id;
                    pkgObj.PackageUrl = item.PackageUrl;
                    pkgObj.LocationAddress = item.Country?.Name ?? "-";
                    pkgObj.LocationId = item.Country?.Id;
                    pkgObj.PkgDtlId = item.Id;
                    pkgObj.IsCruseIncluded = item.IsCruseIncluded;
                    pkgObj.IsTransferIncluded = item.IsTransferIncluded;
                    pkgObj.IsTransportIncluded = item.IsTransportIncluded;
                    pkgObj.IsHotelIncluded = item.IsHotelIncluded;
                    pkgObj.IsMealIncluded = item.IsMealIncluded;
                    pkgObj.PackagePrice = item.PackagePrice;
                    pkgObj.PackagePriceFront = item.PackagePrice.ToString("0.##");
                    pkgObj.PackageName = item.Name;
                    pkgObj.PkgDesc = item.Description?.Length <= 83 ? item.Description : item.Description?.Substring(0, 83) + "..."; ;
                    pkgObj.Description = item.Description;
                    pkgObj.PackageNoOf_DaysNight = item.PackageNoOfDays.ToString() + " Days & " + item.PackageNoOfNights.ToString() + " Nights";
                    pkgObj.PackageNoOfDays = item.PackageNoOfDays;
                    pkgObj.PackageNoOfNights = item.PackageNoOfNights;

                    pkgObj.CountryName = item.Country?.Name ?? "";
                    pkgObj.FromDate = item.FromDate.ToString("dd MMM, yyyy");
                    pkgObj.ToDate = item.ToDate.ToString("dd MMM, yyyy");

                    var imagesObj = item.PackageImages.FirstOrDefault();
                    if (imagesObj != null)
                    {
                        pkgObj.FileOriginalName = imagesObj.OriginalImageName ?? "";
                        pkgObj.FileExtension = imagesObj.ImageExtension;
                        pkgObj.PkgImgId = imagesObj.Id;
                        string RetImageName = "";
                        pkgObj.FilePath = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, imagesObj.ImageName, (UploadSection)imagesObj.ImageSection, imagesObj.OriginalImageName, imagesObj.ImageExtension, SiteKeys.NoImagePath_Square);

                        imagesObj.ImageName = RetImageName;
                    }
                    modelsLst.Add(pkgObj);
                }
            }
            return modelsLst;
        }

        private List<PackageDetailDTO> BindPackageDetailsListData(List<PackageDetail> packageDetails)
        {
            List<PackageDetailDTO> packageDetailLst = new List<PackageDetailDTO>();

            foreach (var item in packageDetails)
            {
                PackageDetailDTO packageDetailDto = new PackageDetailDTO();
                packageDetailDto.PackageId = item.PackageId;
                packageDetailDto.Id = item.Id;
                packageDetailDto.DayIndex = item.DayIndex;
                packageDetailDto.ActivityType = item.ActivityType;
                packageDetailDto.ActivityDate = item.ActivityDate.ToString("dd-MMM-yyyy");
                packageDetailDto.ActivityTime = item.ActivityTime;
                packageDetailDto.ActivityName = item.ActivityName;
                packageDetailDto.LocationAddress = item.LocationAddress;
                packageDetailDto.RequiredInPackageId = item.RequiredInPackageId;
                packageDetailDto.DisplayOrder = item.DisplayOrder;
                packageDetailDto.IsHotelIncludedAct = item.IsHotelIncluded;
                packageDetailDto.IsTransportIncludedAct = item.IsTransportIncluded;
                packageDetailDto.IsMealIncludedAct = item.IsMealIncluded;
                packageDetailDto.IsLunchAct = item.IsLunch;
                packageDetailDto.IsBreakfastAct = item.IsBreakfast;
                packageDetailDto.IsDinnerAct = item.IsDinner;
                packageDetailDto.IsDefaultAct = item.IsDefault;

                var Inclusion_Exclusionlst = _activityIncExcService.GetActivityIncExcMasterList();
                //Activity Inclusion
                List<string> _ActivityInclusionlst = new List<string>();
                List<string> _ActivityExclusionlst = new List<string>();
                var Inclusion = _activityIncExcService.GetActivityIncExcByPackageDetailId(item.Id).Where(x => x.Type == 1);
                if (Inclusion != null)
                {
                    foreach (var inc in Inclusion)
                    {
                        var selectInclusion = Inclusion_Exclusionlst.FirstOrDefault(ut => ut.Id == inc.ActivityIncExcMasterId);
                        _ActivityInclusionlst.Add(selectInclusion?.Name);
                    }

                }
                //Activity Exclusion
                var Exclusion = _activityIncExcService.GetActivityIncExcByPackageDetailId(item.Id).Where(x => x.Type == 2);
                if (Exclusion != null)
                {
                    foreach (var exc in Exclusion)
                    {
                        var selectInclusion = Inclusion_Exclusionlst.FirstOrDefault(ut => ut.Id == exc.ActivityIncExcMasterId);
                        _ActivityExclusionlst.Add(selectInclusion?.Name);
                    }

                }

                packageDetailDto.ActivityInclusionlst = _ActivityInclusionlst;
                packageDetailDto.ActivityExclusionlst = _ActivityExclusionlst;

                //Hotel
                packageDetailDto.PackageHotels = new List<PackageHotelDTO>();
                packageDetailDto.PackageHotels = BindHotelViewData(item.PackageHotels.ToList());

                //Activity Image 
                packageDetailDto.PackageDetailActivityImages = new List<FileUploadDTO>();
                packageDetailDto.PackageDetailActivityImages = BindActivityImageViewData(item.PackageDetailActivityImages.ToList());

                //Activity Video 
                packageDetailDto.PackageDetailActivityVideos = new List<FileUploadDTO>();
                packageDetailDto.PackageDetailActivityVideos = BindActivityVideoViewData(item.PackageDetailActivityVideos.ToList());

                packageDetailLst.Add(packageDetailDto);
            }

            return packageDetailLst;
        }

        private List<PackageHotelDTO> BindHotelViewData(List<PackageHotel> packageHotels)
        {
            List<PackageHotelDTO> packageHotellst = new List<PackageHotelDTO>();
            foreach (var entity in packageHotels)
            {
                PackageHotelDTO packageHtel = new PackageHotelDTO();
                packageHtel.Id = entity.Id;
                packageHtel.PackageDetailId = entity.PackageDetailId;
                packageHtel.HotelId = entity.HotelId;
                packageHtel.Date = entity.Date;
                packageHtel.IsActive = entity.IsActive;
                packageHtel.IsDeleted = entity.IsDeleted;
                packageHtel.CreatedBy = entity.CreatedBy;
                packageHtel.CreatedOn = entity.CreatedOn;
                packageHtel.ModifiedBy = entity.ModifiedBy;
                packageHtel.ModifiedOn = entity.ModifiedOn;
                packageHtel.Hotel = BindPackageHotelDetailsViewData(entity.Hotel);
                packageHotellst.Add(packageHtel);
            }

            return packageHotellst;
        }

        private List<FileUploadDTO> BindActivityImageViewData(List<PackageDetailActivityImage> packageDetailActivityImages)
        {
            List<FileUploadDTO> PackageDetailActivityImagesLst = new List<FileUploadDTO>();
            foreach (var entity in packageDetailActivityImages)
            {
                PackageDetailActivityImagesLst.Add(new FileUploadDTO()
                {
                    EntityId = entity.PackageDetailId,
                    Name = entity.ImageName,
                    FileSize = entity.ImageSize ?? 0,
                    FId = entity.Id,
                    SectionId = 3,   //entity.ImageSection,
                    FileOriginalName = entity.OriginalImageName,
                    FileExtension = entity.ImageExtension,
                    FilePath = entity.GetFilePath(null, (UploadSection)3, x => x.ImageName, x => x.ImageExtension, x => x.OriginalImageName),
                    //Titles = titles
                });
            }
            return PackageDetailActivityImagesLst;
        }
        private List<FileUploadDTO> BindActivityVideoViewData(List<PackageDetailActivityVideo> packageDetailActivityVideos)
        {
            List<FileUploadDTO> PackageDetailActivityVideoLst = new List<FileUploadDTO>();
            foreach (var entity in packageDetailActivityVideos)
            {
                PackageDetailActivityVideoLst.Add(new FileUploadDTO()
                {
                    EntityId = entity.PackageDetailId,
                    Name = entity.VideoName,
                    FileSize = entity.VideoSize ?? 0,
                    FId = entity.Id,
                    SectionId = 3,   //entity.VideoSection,  //#####
                    FileOriginalName = entity.OriginalVideoName,
                    FileExtension = entity.VideoExtension,
                    FilePath = entity.GetFilePath(null, (UploadSection)3, x => x.VideoName, x => x.VideoExtension, x => x.OriginalVideoName),
                    //Titles = titles
                });
            }
            return PackageDetailActivityVideoLst;
        }

        private HotelmodelDTO BindPackageHotelDetailsViewData(Hotel entity)
        {

            HotelmodelDTO packageHotel = new HotelmodelDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Address = entity.Address,
                Phone = entity.Phone,
                HotelUrl = entity.HotelUrl,
                Rating = entity.Rating,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedBy = entity.CreatedBy,
                CreatedOn = entity.CreatedOn,
                ModifiedBy = entity.ModifiedBy,
                ModifiedOn = entity.ModifiedOn,
                HotelImages = BindPackageHotelImageViewData(entity),
                HotelAmenityList = entity.HotelAmenities.Select(x => x.HotelAmenityMaster.Name).ToList()
            };
            return packageHotel;
        }
        private List<HotelImageModelDto> BindPackageHotelImageViewData(Hotel entity)
        {
            List<HotelImageModelDto> hotelImagelst = new List<HotelImageModelDto>();
            if (entity.HotelImages != null)
            {
                foreach (var img in entity.HotelImages)
                {
                    HotelImageModelDto hotelImage = new HotelImageModelDto();
                    hotelImage.Id = img.Id;
                    hotelImage.HotelId = img.HotelId;
                    hotelImage.ImageName = img.ImageName;
                    hotelImage.ImageSize = img.ImageSize;
                    hotelImage.OriginalImageName = img.OriginalImageName;
                    hotelImage.ImageExtension = img.ImageExtension;
                    hotelImage.FilePath = entity.GetFilePath(null, (UploadSection)4, x => img.ImageName, x => img.ImageExtension, x => img.OriginalImageName);
                    hotelImagelst.Add(hotelImage);
                }

            }
            return hotelImagelst;
        }
    }
}
