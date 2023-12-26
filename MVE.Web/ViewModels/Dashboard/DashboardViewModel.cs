using System.Drawing.Printing;
using TCP.Core.Models.Security;

namespace TCP.Web.ViewModels
{
    public class DashboardViewModel
    {
        #region [ Private Properties ]
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private CustomPrincipal CurrentUser { get; set; }

        public void SetEntity(CustomPrincipal currentUser)
        {
            CurrentUser = currentUser;
        }

        public void SetHostingEnvironment(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }
        #endregion
        public List<PackageListViewModel> UpcomingBookingPkgLst { get; set; }
        public List<PackageListViewModel> BookingPkgLst { get; set; }
        public int TotalBookings { get; set; }
        public int TotalSpentBookings { get; set; }
        public IFormFile UserProfileImg { get; set; }
    }
    public class MyBookingDto
    {
        public long PackageId { get; set; }
        public long BookingNo { get; set; }
        public string PackageName { get; set; }
        public string PackageImage { get; set; }
        public string PackageDuration { get; set; }
        public decimal PackagePrice { get; set; }
        public string PackagePriceFront { get; set; }
        public string PackageStartDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string PkgDesc { get; set; }
        public string BookingPerson { get; set; }
        public string PackageUrl { get; set; }
        public int? RatingId{ get; set; }

    }
    public class MyBookingViewModel
    {
        public List<MyBookingDto> UpcomingBooking { get; set; }
        public List<MyBookingDto> AllBooking { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalItem { get; set; }
        public int TotalUpcomingItem { get; set; }
        public int CurrentPageIndexUpcoming { get; set; }
        public string PackageUrl { get; set; }

    }

    public class MyNotificationDto
    {
        public long NotificationId { get; set; }
        public string notificationsTypeName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string CreatedDate { get; set; }
    }

    public class MyNotificationViewModel
    {      
        public List<MyNotificationDto> NotificationList { get; set; }
        public int CurrentPageIndex { get; set; }
        public int TotalItem { get; set; }
        public int TotalPages { get; set; }
        public const int TOTAL_PAGE_BLOCK_MAX_SIZE = 10;
        private int _totalPagesBlockSize = 10;
        public int TotalPagesBlockSize// at a time display paging no.
        {
            get
            {
                return _totalPagesBlockSize;
            }
            set
            {
                if (value <= 0)
                {
                    TotalPagesBlockSize = TotalPagesBlockSize;
                }
                else if (value > TOTAL_PAGE_BLOCK_MAX_SIZE)
                {
                    _totalPagesBlockSize = TOTAL_PAGE_BLOCK_MAX_SIZE;
                }
                else
                {
                    _totalPagesBlockSize = value;
                }
            }
        }
        public int TotalRecords { get; set; }
        private int _pageNo = 1;
        public int PageNo { get; set; }
       
        public bool HasNext => PageNo < TotalPages;
        public bool HasPrev => PageNo > 1;
    }
 
}
