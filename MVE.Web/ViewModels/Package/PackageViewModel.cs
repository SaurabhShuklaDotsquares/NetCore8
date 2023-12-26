using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using TCP.Core.Code;
using TCP.Core.Code.Extensions;
using TCP.Core.Code.LIBS;
using TCP.Core.Models.Security;
using TCP.Data.Models;
using TCP.DataTable.Extension;
using TCP.DataTable.Search;
using TCP.DataTable.Sort;
using TCP.Dto;
using TCP.Web.ViewModels;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace TCP.Web.ViewModels
{
    public class PackageViewModel
    {
        public PackageViewModel() { }
        private CustomPrincipal CurrentUser { get; set; }

        private IHostingEnvironment _HostingEnvironment { get; set; }

        #region [Public Properties]
        public long Id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Country Required")]
        public int CountryId { get; set; }
        [Required(ErrorMessage = "Category Required")]
        public int ThemeId { get; set; }
        public int RegionId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string? Description { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal PricePerChild { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal PackagePrice { get; set; }
        public string PackagePriceFront { get; set; }
        public int NoOfAdults { get; set; }
        public int NoOfChilds { get; set; }
        public int NoOfInfants { get; set; }
        public int PackageNoOfDays { get; set; }
        public int PackageNoOfNights { get; set; }
        public int PackageStatusId { get; set; }
        public int PackageType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public bool IsHotelIncluded { get; set; }

        public bool IsMealIncluded { get; set; }

        public bool IsTransferIncluded { get; set; }

        public bool IsOneWayTransfer { get; set; }

        public bool IsTwoWayTransfer { get; set; }

        public bool IsTransportIncluded { get; set; }

        public bool IsByRoad { get; set; }

        public bool IsByTrain { get; set; }

        public bool IsByFlight { get; set; }


        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Regions { get; set; }
        #endregion [Public Properties]

        #region [Common Properties]
        private TCP.DataTable.DataTables.DataTable DataTablesRequest { get; set; }
        public PackageViewModel(TCP.DataTable.DataTables.DataTable dataTablesRequest)
        {
            DataTablesRequest = dataTablesRequest;
        }
        public bool isComposed { get; set; }
        public List<DataTableRow> GridViewData { get; set; }
        private IEnumerable<Package> _PackageList { get; set; }
        private Package _Package { get; set; }
        public TravellersViewModel travellersmodel { get; set; }
        public GeneralSiteSetting siteSettings { get; set; }

        private IEnumerable<CountryMaster> _AllCountry { get; set; }
        private IEnumerable<Theme> _AllTheme { get; set; }
        private IEnumerable<Region> _AllRegion { get; set; }
        public List<PackageDetailDTO> PackageDetails { get; set; } = new List<PackageDetailDTO>();
        public List<FileUploadDTO> PackageImages { get; set; }
        public List<PackageInclusionDTO> PackageInclusions { get; set; } = new List<PackageInclusionDTO>();
        public List<PackageExclusionDTO> PackageExclusions { get; set; } = new List<PackageExclusionDTO>();
        public List<long> DeletedPackageImages { get; set; } = new List<long>();
        public List<long> DeletedPackageInclusions { get; set; } = new List<long>();
        public List<long> DeletedPackageExclusions { get; set; } = new List<long>();
        public List<long> DeletedPackageDetails { get; set; } = new List<long>();
        public List<long> DeletedPackageDetailActivityImages { get; set; } = new List<long>();
        public List<long> DeletedPackageDetailActivityVideos { get; set; } = new List<long>();
        public string? CountryName { get; internal set; }

        //private IEnumerable<RequiredInPackage> _RequiredInPackages { get; set; }
        //private IEnumerable<Activity> _Activities { get; set; }

        #endregion [Common Properties]

        #region [Package Grid Binding Methods]
        public SearchQuery<Package> FilterGrid(bool? status, int sortIndex, StringValues sortDirection)
        {
            var query = new SearchQuery<Package>();
            if (!string.IsNullOrEmpty(DataTablesRequest.sSearch))
            {
                string sSearch = DataTablesRequest.sSearch.ToLower().Trim();
                query.AddFilter(ad => ad.Name.ToLower().Contains(sSearch)
                || ad.Id.ToString().Contains(sSearch)
                || ad.Country.Name.ToLower().Contains(sSearch)
                || ad.PackagePrice.ToString().Contains(sSearch)
                );
            }

            if (status != null)
            {
                query.AddFilter(ad => ad.IsActive == status);
            }
            query.Take = DataTablesRequest.iDisplayLength;
            query.Skip = DataTablesRequest.iDisplayStart;

            return ShortGrid(query, sortIndex, sortDirection);
        }

        private SearchQuery<Package> ShortGrid(SearchQuery<Package> query, int sortIndex, StringValues sortDirection)
        {
            switch (sortIndex)
            {
                case 2:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, long>(q => q.Id, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 3:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, string>(q => q.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 4:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, string>(q => q.Country.Name, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 5:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, decimal?>(q => q.PackagePrice, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 6:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, int?>(q => q.PackageNoOfDays, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 7:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, DateTime>(q => q.FromDate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 8:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, DateTime>(q => q.ToDate, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 9:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, DateTime>(q => q.CreatedOn, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                case 10:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, bool>(q => q.IsActive, sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending));
                    break;
                default:
                    query.AddSortCriteria(new ExpressionSortCriteria<Package, DateTime?>(q => q.ModifiedOn, SortDirection.Descending));
                    break;
            }
            return query;
        }

        public void SetEntity(IEnumerable<Package> entity)
        {
            _PackageList = entity;
        }

        public void ComposeViewData()
        {
            if (_PackageList == null)
            {
                BindPackageViewData();
            }
            else
            {
                BindGridViewData();
            }
            isComposed = true;
        }

        private void BindGridViewData()
        {
            GridViewData = new List<DataTableRow>();
            int count = DataTablesRequest.iDisplayStart;
            foreach (var item in _PackageList)
            {
                GridViewData.Add(new DataTableRow("rowId" + count.ToString(), "dtrowclass")
                {
                    item.Id.ToString(),
                    (count+1).ToString(),
                    item.Id.ToString(),
                    item.Name,
                    item.Country.Name,
                    item.PackagePrice.ToString(),
                    item.PackageNoOfDays.ToString() + " Days, " + item.PackageNoOfNights.ToString() + " Nights",
                    item.FromDate.ToString(SiteKeys.DateFormatWithMonthName),
                    item.ToDate.ToString(SiteKeys.DateFormatWithMonthName),
                    item.CreatedOn.ToString(SiteKeys.DateFormat),
                    item.IsActive.ToString(),
                });
                count++;
            }
        }

        #endregion [Package Grid Binding Methods]

        #region [Bind Package Data]
        private void BindPackageViewData()
        {
            if (_Package != null)
            {
                Id = _Package.Id;
                Name = _Package.Name;
                CountryId = _Package.CountryId;
                ThemeId = _Package.ThemeId;
                RegionId = _Package.RegionId;
                FromDate = _Package.FromDate.ToString("dd/MM/yyyy");
                ToDate = _Package.ToDate.ToString("dd/MM/yyyy");
                Description = _Package.Description;
                PricePerAdult = _Package.PricePerAdult;
                PricePerChild = _Package.PricePerChild;
                PackagePrice = _Package.PackagePrice;
                NoOfAdults = _Package.NoOfAdults;
                NoOfChilds = _Package.NoOfChilds;
                //PackageNoOfDays = _Package.PackageNoOfDays;
                PackageNoOfDays = _Package.PackageNoOfDays == 0 ? 3 : _Package.PackageNoOfDays;
                PackageNoOfNights = _Package.PackageNoOfNights;
                PackageStatusId = _Package.PackageStatusId;
                PackageType = _Package.PackageType;
                IsActive = _Package.IsActive;
                IsDeleted = _Package.IsDeleted;
                CreatedBy = _Package.CreatedBy;
                CreatedOn = _Package.CreatedOn;
                IsHotelIncluded = _Package.IsHotelIncluded;
                IsMealIncluded = _Package.IsMealIncluded;
                IsTransferIncluded = _Package.IsTransferIncluded;
                IsTransportIncluded = _Package.IsTransportIncluded;
                //ModifiedBy = _Package.ModifiedBy;
                //ModifiedOn = _Package.ModifiedOn;


                BindPackageDetailViewData();
                BindPackageImageViewData();
                BindPackageInclusionViewData();
                BindPackageExclusionViewData();
            }
        }

        private void BindPackageInclusionViewData()
        {
            PackageInclusions = new List<PackageInclusionDTO>();
            foreach (var item in _Package.PackageInclusions)
            {
                BindPackageInclusionViewData(item);
            }
        }

        private void BindPackageInclusionViewData(PackageInclusion entity)
        {
            var inclusion = new PackageInclusionDTO()
            {
                PackageId = entity.PackageId,
                Id = entity.Id,
                DisplayOrder = entity.DisplayOrder,
                IsDeleted = entity.IsDeleted,
                IsActive = entity.IsActive,
                Name = entity.Name,
            };

            PackageInclusions.Add(inclusion);
        }

        private void BindPackageExclusionViewData()
        {
            PackageExclusions = new List<PackageExclusionDTO>();
            foreach (var item in _Package.PackageExclusions)
            {
                BindPackageExclusionViewData(item);
            }
        }

        private void BindPackageExclusionViewData(PackageExclusion entity)
        {
            var exclusion = new PackageExclusionDTO()
            {
                PackageId = entity.PackageId,
                Id = entity.Id,
                DisplayOrder = entity.DisplayOrder,
                IsDeleted = entity.IsDeleted,
                IsActive = entity.IsActive,
                Name = entity.Name,
            };

            PackageExclusions.Add(exclusion);
        }

        private void BindPackageImageViewData()
        {
            PackageImages = new List<FileUploadDTO>();
            foreach (var item in _Package.PackageImages.OrderByDescending(x => x.ModifiedDate))
            {
                BindPackageImageViewData(item);
            }
        }

        private void BindPackageImageViewData(PackageImage packageImage)
        {
            //List<FileUploadTitleDTO> titles = new List<FileUploadTitleDTO>();
            //foreach (var item in accommodationImage.AccommodationImageTitle)
            //{
            //    titles.Add(new FileUploadTitleDTO()
            //    {
            //        FileUploadId = item.AccommodationImageId,
            //        Id = item.Id,
            //        LanguageId = item.LanguageId,
            //        Title = item.Title,
            //        Description = item.Description,
            //    });
            //}

            PackageImages.Add(new FileUploadDTO()
            {
                EntityId = packageImage.PackageId,
                Name = packageImage.ImageName,
                FileSize = packageImage.ImageSize ?? 0,
                FId = packageImage.Id,
                SectionId = packageImage.ImageSection,
                FileOriginalName = packageImage.OriginalImageName,
                FileExtension = packageImage.ImageExtension,
                FilePath = packageImage.GetFilePath(null, (UploadSection)packageImage.ImageSection, x => x.ImageName, x => x.ImageExtension, x => x.OriginalImageName),
                //Titles = titles,
                ImageOrder = packageImage.ImageOrder
            });
        }

        private void BindPackageDetailViewData()
        {
            PackageDetails = new List<PackageDetailDTO>();
            foreach (var item in _Package.PackageDetails)
            {
                BindPackageDetailViewData(item);
            }
        }

        private void BindPackageDetailViewData(PackageDetail entity)
        {
            PackageDetailDTO packageDetail = new PackageDetailDTO()
            {
                PackageId = entity.PackageId,
                Id = entity.Id,
                DayIndex = entity.DayIndex,
                ActivityType = entity.ActivityType,
                ActivityDate = entity.ActivityDate.ToString("dd-MMM-yyyy"),
                ActivityTime = entity.ActivityTime,
                ActivityName = entity.ActivityName,
                LocationAddress = entity.LocationAddress,
                RequiredInPackageId = entity.RequiredInPackageId,
                DisplayOrder = entity.DisplayOrder,
                IsHotelIncludedAct = entity.IsHotelIncluded,
                IsTransportIncludedAct = entity.IsTransportIncluded,
                IsMealIncludedAct = entity.IsMealIncluded,
                IsLunchAct = entity.IsLunch,
                IsBreakfastAct = entity.IsBreakfast,
                IsDinnerAct = entity.IsDinner,
                IsDefaultAct = entity.IsDefault,
            };
            
            BindPackageDetailActivityImageViewData(entity, packageDetail);
            BindPackageDetailActivityVideoViewData(entity, packageDetail);
            BindPackageHotelViewData(entity, packageDetail);
            PackageDetails.Add(packageDetail);
        }


        #region [Hotel and Hotel Images--- Web]

        //bind all package hotel details of package
        private void BindPackageHotelViewData(PackageDetail entity, PackageDetailDTO model)
        {
            model.PackageHotels = new List<PackageHotelDTO>();
            foreach (var item in entity.PackageHotels)
            {
                BindPackageHotelViewData(item, model);
            }
        }

        private void BindPackageHotelViewData(PackageHotel entity, PackageDetailDTO model)
        {

            model.PackageHotels.Add(new PackageHotelDTO()
            {
                PackageDetailId = entity.PackageDetailId,
                HotelId = entity.HotelId,
                HotelName = entity.Hotel?.Name,
                HotelDesc = entity.Hotel?.Description ?? "-",
                HotelRating = entity.Hotel?.Rating ?? 0,
                HotelAddress = entity.Hotel?.Address ?? "",
                PackageHotelImages = BindPackageHotelImageViewData1(entity?.Hotel)
            });

        }

        private List<FileUploadDTO> BindPackageHotelImageViewData1(Hotel entity)
        {
            List<FileUploadDTO> lst = new List<FileUploadDTO>();
            foreach (var item in entity.HotelImages)
            {
                FileUploadDTO fileUploadDTO = new FileUploadDTO();
                fileUploadDTO.Name = item.ImageName;
                fileUploadDTO.FileSize = item.ImageSize ?? 0;
                fileUploadDTO.FId = item.Id;
                fileUploadDTO.SectionId = 3;
                fileUploadDTO.FileOriginalName = item.OriginalImageName ?? "";
                fileUploadDTO.FileExtension = item.ImageExtension;
                //fileUploadDTO.FilePath = item.GetFilePath(null, (UploadSection)3, x => x.ImageName, x => x.ImageExtension, x => x.OriginalImageName);
                //fileUploadDTO.FilePath = CommonFileViewModel.GetFilePathByAdmin(UploadSection.HotelImage, item.ImageName, item.OriginalImageName, item.ImageExtension);

                string RetImageName = "";
                fileUploadDTO.FilePath = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, item.ImageName, UploadSection.HotelImage, item.OriginalImageName, item.ImageExtension, SiteKeys.NoImagePath_Square);
                //item.ImageName = RetImageName;
                //********

                //*******

                lst.Add(fileUploadDTO);
            }
            return lst;

        }


        #endregion

        private void BindPackageDetailActivityImageViewData(PackageDetail entity, PackageDetailDTO model)
        {
            model.PackageDetailActivityImages = new List<FileUploadDTO>();
            foreach (var item in entity.PackageDetailActivityImages)
            {
                BindPackageDetailActivityImageViewData(item, model);
            }
        }
        private void BindPackageDetailActivityVideoViewData(PackageDetail entity, PackageDetailDTO model)
        {
            model.PackageDetailActivityVideos = new List<FileUploadDTO>();
            foreach (var item in entity.PackageDetailActivityVideos)
            {
                BindPackageDetailActivityVideoViewData(item, model);
            }
        }

        private void BindPackageDetailActivityImageViewData(PackageDetailActivityImage entity, PackageDetailDTO model)
        {
            string RetImageName = "";
            string file_Path = CommonFileViewModel.GetFilePathByAdmin(ref RetImageName, entity.ImageName, UploadSection.PackageDetailActivity, entity.OriginalImageName, entity.ImageExtension, SiteKeys.NoImagePath_Square);
            //imagesObj.ImageName = RetImageName;

            model.PackageDetailActivityImages.Add(new FileUploadDTO()
            {
                EntityId = entity.PackageDetailId,
                Name = entity.ImageName ?? "",
                FileSize = entity.ImageSize ?? 0,
                FId = entity.Id,
                SectionId = 3,   //entity.ImageSection,
                FileOriginalName = entity.OriginalImageName ?? "",
                FileExtension = entity.ImageExtension ?? "",
                //FilePath = entity.GetFilePath(null, (UploadSection)3, x => x.ImageName, x => x.ImageExtension, x => x.OriginalImageName),
                //FilePath = CommonFileViewModel.GetFilePathByAdmin(UploadSection.PackageDetailActivity, entity.ImageName, entity.OriginalImageName, entity.ImageExtension),
                FilePath = file_Path,


            });
        }

        private void BindPackageDetailActivityVideoViewData(PackageDetailActivityVideo entity, PackageDetailDTO model)
        {
            string RetVideoName = "";
            string file_Path = CommonFileViewModel.GetFilePathByAdmin(ref RetVideoName, entity.VideoName, UploadSection.PackageDetailActivity, entity.OriginalVideoName, entity.VideoExtension, SiteKeys.NoImagePath_Square);
            //imagesObj.ImageName = RetImageName;

            model.PackageDetailActivityVideos.Add(new FileUploadDTO()
            {
                EntityId = entity.PackageDetailId,
                Name = entity.VideoName ?? "",
                FileSize = entity.VideoSize ?? 0,
                FId = entity.Id,
                SectionId = 3,   //entity.VideoSection,  //#####
                FileOriginalName = entity.OriginalVideoName ?? "",
                FileExtension = entity.VideoExtension ?? "",
                //FilePath = entity.GetFilePath(null, (UploadSection)3, x => x.VideoName, x => x.VideoExtension, x => x.OriginalVideoName),
                //FilePath = CommonFileViewModel.GetFilePathByAdmin(UploadSection.PackageDetailActivity, entity.VideoName, entity.OriginalVideoName, entity.VideoExtension),
                FilePath = file_Path,


            });
        }

        public void SetEntity(CustomPrincipal currentUser)
        {
            CurrentUser = currentUser;
        }
        public void SetHostingEnvironment(IHostingEnvironment hostingEnvironment)
        {
            _HostingEnvironment = hostingEnvironment;
        }
        public void SetEntity(IEnumerable<CountryMaster> countries)
        {
            _AllCountry = countries;
            BindContryData();
        }
        public void SetEntity(IEnumerable<Theme> categories)
        {
            _AllTheme = categories;
            BindThemeData();
        }
        public void SetEntity(IEnumerable<Region> regions)
        {
            _AllRegion = regions;
            BindRegionData();
        }
        public void SetEntity(Package entity)
        {
            _Package = entity;
        }
        #endregion [Bind Package Data]

        #region [Common Data Binding]

        private void BindContryData()
        {
            if (_AllCountry != null)
            {
                Countries = _AllCountry.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}", Selected = c.Id == CountryId }).ToList();
                Countries.Insert(0, new SelectListItem() { Text = "-- Select --" });
            }
        }
        private void BindThemeData()
        {
            if (_AllTheme != null)
            {
                Categories = _AllTheme.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}", Selected = c.Id == ThemeId }).ToList();
                Categories.Insert(0, new SelectListItem() { Text = "-- Select --" });
            }
        }
        private void BindRegionData()
        {
            if (_AllRegion != null)
            {
                Regions = _AllRegion.Select(c => new SelectListItem() { Value = c.Id.ToString(), Text = $"{c.Name}", Selected = c.Id == RegionId }).ToList();
                Regions.Insert(0, new SelectListItem() { Text = "-- Select --" });
            }
        }
        #endregion [Common Data Binding]

        #region [Bind Entity Data to Save]
        public Package ComposeEntityData()
        {
            BindPackageEntityData();
            isComposed = true;
            return _Package;
        }

        private void BindPackageEntityData()
        {
            if (_Package != null)
            {
                _Package.Id = Id;
                _Package.Name = Name;
                _Package.CountryId = CountryId;
                _Package.ThemeId = ThemeId;
                _Package.RegionId = RegionId;
                _Package.FromDate = DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                _Package.ToDate = DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                _Package.Description = Description;
                _Package.PricePerAdult = PricePerAdult;
                _Package.PricePerChild = PricePerChild;
                _Package.PackagePrice = PackagePrice;
                _Package.NoOfAdults = NoOfAdults;
                _Package.NoOfChilds = NoOfChilds;
                _Package.PackageNoOfDays = PackageNoOfDays;
                _Package.PackageNoOfNights = PackageNoOfNights;
                _Package.PackageStatusId = PackageStatusId;
                _Package.PackageType = PackageType;
                _Package.IsActive = IsActive;
                _Package.IsDeleted = IsDeleted;
                if (_Package.Id > 0)
                {
                    _Package.ModifiedBy = CurrentUser.Id;
                    _Package.ModifiedOn = DateTime.UtcNow;
                }
                else
                {
                    _Package.CreatedBy = CurrentUser.Id;
                    _Package.CreatedOn = DateTime.UtcNow;
                    _Package.ModifiedBy = CurrentUser.Id;
                    _Package.ModifiedOn = DateTime.UtcNow;
                }

                BindPackageDetailEntityData();
                BindPackageImageEntityData();
                BindPackageInclusionEntityData();
                BindPackageExclusionEntityData();
            }
        }

        private void BindPackageDetailEntityData()
        {
            _Package.PackageDetails = _Package.PackageDetails ?? new List<PackageDetail>();
            PackageDetails = PackageDetails ?? new List<PackageDetailDTO>();
            DeletedPackageDetails = _Package.PackageDetails.Where(x => !PackageDetails.Any(t => t.Id == x.Id)).Select(x => x.Id).ToList();

            foreach (var item in PackageDetails)
            {
                PackageDetail packageDetail = _Package.PackageDetails.FirstOrDefault(x => x.Id == item.Id) ?? new PackageDetail();

                BindPackageDetailEntityData(packageDetail, item);
                BindPackageDetailActivityImageEntityData(packageDetail, item);
                if (packageDetail.Id == 0)
                {
                    _Package.PackageDetails.Add(packageDetail);
                }
            }
        }

        private void BindPackageDetailActivityImageEntityData(PackageDetail packageDetail, PackageDetailDTO model)
        {
            packageDetail.PackageDetailActivityImages = packageDetail.PackageDetailActivityImages ?? new List<PackageDetailActivityImage>();
            model.PackageDetailActivityImages = model.PackageDetailActivityImages ?? new List<FileUploadDTO>();
            List<long> oldIds = packageDetail.PackageDetailActivityImages.Select(t => t.Id).ToList();
            var deletedImages = packageDetail.PackageDetailActivityImages.Where(x => !model.PackageDetailActivityImages.Any(t => t.FId == x.Id)).Select(x => x.Id).ToList();
            DeletedPackageDetailActivityImages.AddRange(deletedImages);

            FileUploadDTO newUploadedFile = null;
            int itemIndex = 0;
            foreach (var item in model.PackageDetailActivityImages.Where(t => !string.IsNullOrWhiteSpace(t.FileStreams)))
            {
                itemIndex = model.PackageDetailActivityImages.IndexOf(item);
                newUploadedFile = item.SaveFileToDirectory(_HostingEnvironment);
                model.PackageDetailActivityImages[itemIndex].Name = newUploadedFile.Name;
                model.PackageDetailActivityImages[itemIndex].FileExtension = newUploadedFile.FileExtension;
                model.PackageDetailActivityImages[itemIndex].FileSize = newUploadedFile.FileSize;
                model.PackageDetailActivityImages[itemIndex].SectionId = newUploadedFile.SectionId;
                model.PackageDetailActivityImages[itemIndex].FileOriginalName = newUploadedFile.FileOriginalName;
            }

            foreach (var item in model.PackageDetailActivityImages)
            {
                PackageDetailActivityImage packageDetailActivityImage = item.FId > 0 ? packageDetail.PackageDetailActivityImages.FirstOrDefault(t => t.Id == item.FId) ?? new PackageDetailActivityImage() : new PackageDetailActivityImage();

                packageDetailActivityImage.PackageDetailId = packageDetail.Id;
                packageDetailActivityImage.CreatedDate = item.FId > 0 ? packageDetailActivityImage.CreatedDate : DateTime.UtcNow;
                packageDetailActivityImage.ImageName = item.Name;
                packageDetailActivityImage.ImageExtension = item.FileExtension;
                packageDetailActivityImage.ImageSize = item.FileSize;
                packageDetailActivityImage.Id = item.FId;
                //packageDetailActivityImage.ImageSection = item.SectionId;
                packageDetailActivityImage.ModifiedDate = DateTime.UtcNow;
                packageDetailActivityImage.OriginalImageName = item.FileOriginalName;

                if (packageDetailActivityImage.Id == 0)
                {
                    packageDetail.PackageDetailActivityImages.Add(packageDetailActivityImage);
                }
            }

            foreach (var id in oldIds)
            {
                if (!model.PackageDetailActivityImages.Any(t => t.FId == id))
                {
                    PackageDetailActivityImage packageDetailActivityImage = packageDetail.PackageDetailActivityImages.FirstOrDefault(t => t.Id == id);
                    packageDetailActivityImage.MoveFileToDeleted(_HostingEnvironment, (UploadSection)3, x => x.ImageName, x => x.ImageName, x => x.OriginalImageName);
                }
            }
        }

        private void BindPackageDetailActivityVideoEntityData(PackageDetail packageDetail, PackageDetailDTO model)
        {
            packageDetail.PackageDetailActivityVideos = packageDetail.PackageDetailActivityVideos ?? new List<PackageDetailActivityVideo>();
            model.PackageDetailActivityVideos = model.PackageDetailActivityVideos ?? new List<FileUploadDTO>();
            List<long> oldIds = packageDetail.PackageDetailActivityVideos.Select(t => t.Id).ToList();
            var deletedVideos = packageDetail.PackageDetailActivityVideos.Where(x => !model.PackageDetailActivityVideos.Any(t => t.FId == x.Id)).Select(x => x.Id).ToList();
            DeletedPackageDetailActivityVideos.AddRange(deletedVideos);

            FileUploadDTO newUploadedFile = null;
            int itemIndex = 0;
            foreach (var item in model.PackageDetailActivityVideos.Where(t => !string.IsNullOrWhiteSpace(t.FileStreams)))
            {
                itemIndex = model.PackageDetailActivityVideos.IndexOf(item);
                newUploadedFile = item.SaveFileToDirectory(_HostingEnvironment);
                model.PackageDetailActivityVideos[itemIndex].Name = newUploadedFile.Name;
                model.PackageDetailActivityVideos[itemIndex].FileExtension = newUploadedFile.FileExtension;
                model.PackageDetailActivityVideos[itemIndex].FileSize = newUploadedFile.FileSize;
                model.PackageDetailActivityVideos[itemIndex].SectionId = newUploadedFile.SectionId;
                model.PackageDetailActivityVideos[itemIndex].FileOriginalName = newUploadedFile.FileOriginalName;
            }

            foreach (var item in model.PackageDetailActivityVideos)
            {
                PackageDetailActivityVideo packageDetailActivityVideo = item.FId > 0 ? packageDetail.PackageDetailActivityVideos.FirstOrDefault(t => t.Id == item.FId) ?? new PackageDetailActivityVideo() : new PackageDetailActivityVideo();

                packageDetailActivityVideo.PackageDetailId = packageDetail.Id;
                packageDetailActivityVideo.CreatedDate = item.FId > 0 ? packageDetailActivityVideo.CreatedDate : DateTime.UtcNow;
                packageDetailActivityVideo.VideoName = item.Name;
                packageDetailActivityVideo.VideoExtension = item.FileExtension;
                packageDetailActivityVideo.VideoSize = item.FileSize;
                packageDetailActivityVideo.Id = item.FId;
                //packageDetailActivityVideo.VideoSection = item.SectionId;
                packageDetailActivityVideo.ModifiedDate = DateTime.UtcNow;
                packageDetailActivityVideo.OriginalVideoName = item.FileOriginalName;

                if (packageDetailActivityVideo.Id == 0)
                {
                    packageDetail.PackageDetailActivityVideos.Add(packageDetailActivityVideo);
                }
            }

            foreach (var id in oldIds)
            {
                if (!model.PackageDetailActivityVideos.Any(t => t.FId == id))
                {
                    PackageDetailActivityVideo packageDetailActivityVideo = packageDetail.PackageDetailActivityVideos.FirstOrDefault(t => t.Id == id);
                    packageDetailActivityVideo.MoveFileToDeleted(_HostingEnvironment, (UploadSection)3, x => x.VideoName, x => x.VideoName, x => x.OriginalVideoName);
                }
            }
        }

        private void BindPackageDetailEntityData(PackageDetail packageDetail, PackageDetailDTO model)
        {
            packageDetail.Id = model.Id;
            packageDetail.PackageId = model.PackageId;
            packageDetail.DayIndex = model.DayIndex;
            packageDetail.ActivityType = model.ActivityType;
            packageDetail.ActivityDate = DateTime.ParseExact(model.ActivityDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            packageDetail.ActivityTime = model.ActivityTime;
            packageDetail.ActivityName = model.ActivityName;
            packageDetail.LocationAddress = model.LocationAddress;
            packageDetail.RequiredInPackageId = 1;
            packageDetail.DisplayOrder = model.DisplayOrder;
            packageDetail.IsHotelIncluded = model.IsHotelIncludedAct;
            packageDetail.IsTransportIncluded = model.IsTransportIncludedAct;
            packageDetail.IsMealIncluded = model.IsMealIncludedAct;
            packageDetail.IsLunch = model.IsLunchAct;
            packageDetail.IsBreakfast = model.IsBreakfastAct;
            packageDetail.IsDinner = model.IsDinnerAct;
            if (model.Id > 0)
            {
                packageDetail.ModifiedBy = CurrentUser.Id;
                packageDetail.ModifiedOn = DateTime.UtcNow;
            }
            else
            {
                packageDetail.CreatedBy = CurrentUser.Id;
                packageDetail.CreatedOn = DateTime.UtcNow;
                packageDetail.ModifiedBy = CurrentUser.Id;
                packageDetail.ModifiedOn = DateTime.UtcNow;
            }
        }

        private void BindPackageInclusionEntityData()
        {
            _Package.PackageInclusions = _Package.PackageInclusions ?? new List<PackageInclusion>();
            PackageInclusions = PackageInclusions ?? new List<PackageInclusionDTO>();

            DeletedPackageInclusions = _Package.PackageInclusions.Where(x => !PackageInclusions.Any(t => t.Id == x.Id)).Select(x => x.Id).ToList();
            foreach (var item in PackageInclusions)
            {
                PackageInclusion packageInclusion = item.Id > 0 ? _Package.PackageInclusions.FirstOrDefault(t => t.Id == item.Id) : new PackageInclusion();

                BindPackageInclusionEntityData(packageInclusion, item);

                if (packageInclusion.Id == 0)
                {
                    _Package.PackageInclusions.Add(packageInclusion);
                }
            }
        }

        private void BindPackageInclusionEntityData(PackageInclusion entity, PackageInclusionDTO model)
        {
            entity.PackageId = model.PackageId;
            entity.Id = model.Id;
            entity.DisplayOrder = model.DisplayOrder;
            entity.IsDeleted = false; // model.IsDeleted;
            entity.IsActive = true;   // model.IsActive;
            entity.Name = model.Name;
        }

        private void BindPackageExclusionEntityData()
        {
            _Package.PackageExclusions = _Package.PackageExclusions ?? new List<PackageExclusion>();
            PackageExclusions = PackageExclusions ?? new List<PackageExclusionDTO>();

            DeletedPackageExclusions = _Package.PackageExclusions.Where(x => !PackageExclusions.Any(t => t.Id == x.Id)).Select(x => x.Id).ToList();
            foreach (var item in PackageExclusions.Where(x => !string.IsNullOrEmpty(x.Name)))
            {
                PackageExclusion packageExclusion = item.Id > 0 ? _Package.PackageExclusions.FirstOrDefault(t => t.Id == item.Id) : new PackageExclusion();

                BindPackageExclusionEntityData(packageExclusion, item);

                if (packageExclusion.Id == 0)
                {
                    _Package.PackageExclusions.Add(packageExclusion);
                }
            }
        }

        private void BindPackageExclusionEntityData(PackageExclusion entity, PackageExclusionDTO model)
        {
            entity.PackageId = model.PackageId;
            entity.Id = model.Id;
            entity.DisplayOrder = model.DisplayOrder;
            entity.IsDeleted = false; // model.IsDeleted;
            entity.IsActive = true;   // model.IsActive;
            entity.Name = model.Name;
        }

        private void BindPackageImageEntityData()
        {
            _Package.PackageImages = _Package.PackageImages ?? new List<PackageImage>();
            PackageImages = PackageImages ?? new List<FileUploadDTO>();
            List<long> oldIds = _Package.PackageImages.Select(t => t.Id).ToList();
            DeletedPackageImages = _Package.PackageImages.Where(x => !PackageImages.Any(t => t.FId == x.Id)).Select(x => x.Id).ToList();

            //var currentImageTitles = AccommodationImages.Where(x => x.Titles != null).SelectMany(x => x.Titles.Select(t => t.Id)).ToArray();
            //DeletedAccommodationImageTitles = _Accommodation.AccommodationImage.SelectMany(x => x.AccommodationImageTitle).Where(x => !currentImageTitles.Contains(x.Id)).Select(x => x.Id).ToList();

            FileUploadDTO newUploadedFile = null;
            int itemIndex = 0;
            foreach (var item in PackageImages.Where(t => !string.IsNullOrWhiteSpace(t.FileStreams)))
            {
                itemIndex = PackageImages.IndexOf(item);
                newUploadedFile = item.SaveFileToDirectory(_HostingEnvironment);
                PackageImages[itemIndex].Name = newUploadedFile.Name;
                PackageImages[itemIndex].FileExtension = newUploadedFile.FileExtension;
                PackageImages[itemIndex].FileSize = newUploadedFile.FileSize;
                PackageImages[itemIndex].SectionId = newUploadedFile.SectionId;
                PackageImages[itemIndex].FileOriginalName = newUploadedFile.FileOriginalName;
                PackageImages[itemIndex].ImageOrder = newUploadedFile.ImageOrder;
            }

            foreach (var item in PackageImages.Where(x => !string.IsNullOrEmpty(x.Name) && x.FileSize > 0))
            {
                PackageImage packageImage = item.FId > 0 ? _Package.PackageImages.FirstOrDefault(t => t.Id == item.FId) ?? new PackageImage() : new PackageImage();

                DateTime modifiedDate = DateTime.UtcNow;

                //if (packageImage.Id > 0 && packageImage.FileName == item.Name && packageImage.ImageSection == item.SectionId)
                //{
                //    if (packageImage.AccommodationImageTitle?.FirstOrDefault()?.Title == item.Titles?.FirstOrDefault()?.Title)
                //    {
                //        modifiedDate = accommodationImage.ModifiedDate;
                //    }
                //}

                packageImage.PackageId = _Package.Id;
                packageImage.CreatedDate = item.FId > 0 ? packageImage.CreatedDate : DateTime.UtcNow;
                packageImage.ImageName = item.Name;
                packageImage.ImageExtension = item.FileExtension;
                packageImage.ImageSize = item.FileSize;
                packageImage.Id = item.FId;
                packageImage.ImageSection = item.SectionId;
                packageImage.ModifiedDate = modifiedDate;
                packageImage.OriginalImageName = item.FileOriginalName;
                packageImage.ImageOrder = item.ImageOrder;

                //item.Titles = item.Titles ?? new List<FileUploadTitleDTO>();
                //foreach (var _title in item.Titles)
                //{
                //    if (!string.IsNullOrWhiteSpace(_title.Title))
                //    {
                //        AccommodationImageTitle imageTitle = _title.Id > 0 ? accommodationImage.AccommodationImageTitle.FirstOrDefault(x => x.Id == _title.Id) : new AccommodationImageTitle();
                //        imageTitle.LanguageId = _title.LanguageId;
                //        imageTitle.Title = _title.Title;
                //        imageTitle.Description = _title.Description;
                //        imageTitle.AccommodationImageId = accommodationImage.Id;

                //        if (imageTitle.Id == 0)
                //        {
                //            accommodationImage.AccommodationImageTitle.Add(imageTitle);
                //        }
                //    }
                //}

                if (packageImage.Id == 0)
                {
                    _Package.PackageImages.Add(packageImage);
                }
            }


            foreach (var id in oldIds)
            {
                if (!PackageImages.Any(t => t.FId == id))
                {
                    PackageImage packageImage = _Package.PackageImages.FirstOrDefault(t => t.Id == id);
                    packageImage.MoveFileToDeleted(_HostingEnvironment, (UploadSection)packageImage.ImageSection, x => x.ImageName, x => x.ImageName, x => x.OriginalImageName);
                }
            }
        }

        //public void SetEntity(IEnumerable<RequiredInPackage> entity)
        //{
        //    _RequiredInPackages = entity;
        //}

        //public void SetEntity(IEnumerable<Activity> activities)
        //{
        //    _Activities = activities;
        //}
        #endregion [Bind Entity Data to Save]
    }
}
