using System;
using MVE.Core.Models.Security;
using MVE.Data.Models;

namespace MVE.Admin.ViewModels
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

        public long TotalUser { get; set; }
        public long TotalActiveCategories { get; set; }
        public long TotalBookings { get; set; }
        public long TotalUpcomingBookings { get; set; }
        public decimal TotalRevenue { get; set; }

        public long CTotalPendingEnquiries { get; set; }
        public long CTotalCompletedEnquiries { get; set; }
        public long GTotalPendingEnquiries { get; set; }
        public long GTotalResolvedEnquiries { get; set; }

        #endregion
    }

    public class UserChartDTO
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
    public class BookingChartDTO
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
    public class EarningChartDTO
    {
        public string Key { get; set; }
        public decimal Value { get; set; }
    }
    public class CountrywiseBookingDTO
    {
        public int CountryId { get; set; }
        public decimal Amount { get; set; }
    }
}
