using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Code.LIBS
{
    public enum MessageType
    {
        Warning,
        Success,
        Danger,
        Info
    }
    public enum RoleType
    {
        [Description("SuperAdmin")]
        Administrator = 1,
        [Description("Admin")]
        Admin = 2,
    }

    public enum ModalSize
    {
        Small,
        Large,
        Medium,
        XLarge
    }
    public enum ServerFileTypes
    {
        Image = 1,
        Video = 2,
        XML = 3,
        Json = 4
    }
    public enum PackageRequestStatus
    {
        Pending = 0, InProgress = 1, Feedback = 2, Completed = 3, Cancelled = 4
    }
    public enum QuoteRequestStatus
    {
        Pending = 0, InProgress = 1, Feedback = 2, Completed = 3, Cancelled = 4
    }

    public enum BookingStatus
    {
        Pending = 0, InProgress = 1, Confirmed = 2, Cancelled = 3
    }

    public enum DepartureDateType
    {
        Fixed = 1,
        Flexible = 2,
        Anytime = 3
    }

    public enum ImageFor
    {
        [Description("Home")]
        Home = 1

    }
    public enum ImageType
    {
        [Description("VisaGuide")]
        VisaGuide = 1
    }

    public enum HotelRating
    {
        [Description("1 Star")]
        Star1 = 1,
        [Description("2 Star")]
        Star2 = 2,
        [Description("3 Star")]
        Star3 = 3,
        [Description("4 Star")]
        Star4 = 4,
        [Description("5 Star")]
        Star5 = 5
    }
    //public enum IwillBookIn
    //{
    //    [Description("In Next 2-3 Days")]
    //    InNext2_3Days = 1,
    //    [Description("In This Week")]
    //    InThisWeek = 2,
    //    [Description("In This Month")]
    //    InThisMonth = 3,
    //    [Description("Later Sometime")]
    //    LaterSometime = 4,
    //    [Description("Just Checking Price")]
    //    JustCheckingPrice = 5
    //}
    public enum PricedPkgInBudget
    {
        [Description("$1000")]
        LessThan1K = 1,
        [Description("$2000 - $4000")]
        From2Kto4K = 2,
        [Description("$4000 - $6000")]
        From4Kto6K = 3,
        [Description("$6000 - $8000")]
        From6Kto8K = 4,
        [Description("$9000")]
        GreaterThan9K = 5
    }
    public enum PricedPkgInDuration
    {
        [Description("1-3 Days")]
        Days1to3 = 1,
        [Description("4-6 Days")]
        Days4to6 = 2,
        [Description("7-9 Days")]
        Days7to9 = 3,
        [Description("10-12 Days")]
        Days10to12 = 4,
        [Description("13 Days or more")]
        GreaterThan13days = 5
    }
    public enum PkgListingByDuration
    {
        //[Description("All Duration")]
        //AllDuration = 0,
        [Description("1-3 Days")]
        Days1to3 = 1,
        [Description("4-6 Days")]
        Days4to6 = 2,
        [Description("7-9 Days")]
        Days7to9 = 3,
        [Description("10-12 Days")]
        Days10to12 = 4,
        [Description("13 Days or more")]
        GreaterThan13days = 5

    }
    public enum PkgInclusions
    {
        [Description("Meal")]
        Meal = 1,
        [Description("Cruise")]
        Cabs = 2,
        [Description("Transport")]
        Flights = 3,
        [Description("Hotels")]
        Hotels = 4,
        [Description("Transfers")]
        Transfers = 5,
        //[Description("All of the above")]
        //AllofTheAbove = 6
    }
    public enum TicketStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Resolved")]
        Resolved = 2
    }
    public enum NotificationSentType
    {
        [Description("Email Only")]
        EmailOnly = 1,
        [Description("Notification Only")]
        NotificationOnly = 2,
        [Description("Email & Notification")]
        Email_Notification = 3
    }
    public enum ReportType
    {
        Total_Bookings = 1,
        Total_UpcomingBookings = 2,
        Total_Earning = 3,
        Total_Users = 4,
        Total_CustomizePackageEnquiries = 5,
        Total_PlanMyHolidayEnquiries = 6
    }

    public enum PaymentTypeEnum
    {

        Card = 1,
        Cash = 2,
        Wallet = 3
    }
}
