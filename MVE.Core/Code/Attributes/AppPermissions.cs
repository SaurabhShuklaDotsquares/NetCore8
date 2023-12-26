using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Code.Attributes
{
    public enum AppPermissions : int
    {
        [Description("Pages")]
        Pages = 1,

        [Description("Admin Users")]
        Pages_Administration_AdminUsers = 1, //Page id

        [Description("Roles")]
        Pages_Administration_Roles = 2,

        [Description("Manage Holiday Packages")]
        Pages_Administration_Packages = 3,

        [Description("Manage Category")]
        Pages_Administration_Country = 4,

        [Description("Manage User")]
        Pages_Administration_ManageUser = 5,

        [Description("Customize Package")]
        Pages_Administration_CustomizePackage = 6,

        [Description("Plan My Holiday")]
        Pages_Administration_PlanMyHoliday = 7,

        [Description("Change Password")]
        Pages_Administration_ChangePassword = 8,

        [Description("Manage Category")]
        Pages_Administration_ManageCategory = 9,

        [Description("Manage Inclusion/Exclusion")]
        Pages_Administration_ManageInclusionExclusion = 10,

        [Description("Manage Contact Us")]
        Pages_Administration_ManageContactUs = 11,

        [Description("Manage Rating & Reviews")]
        Pages_Administration_ManageRatingAndReviews = 12,

        [Description("Manage Report")]
        Pages_Administration_ManageReport = 13,

        [Description("Manage Bookings")]
        Pages_Administration_ManageBookings = 14,

        [Description("Manage Notifications")]
        Pages_Administration_ManageNotifications = 15,

        [Description("Manage Banner")]
        Pages_Administration_ManageBanner = 16,


        [Description("Static Page")]
        Pages_Administration_StaticPage = 17,

        [Description("Manage Earning")]
        Pages_Administration_ManageEarning = 18,

        [Description("PlanMyHolidayQuestions")]
        Pages_Administration_PlanMyHolidayQuestions = 19,

        [Description("Settings")]
        Pages_Administration_Settings = 20,

        [Description("Destination Manager")]
        Pages_Administration_DestinationManager = 21,

        [Description("Visa Guide Manager")]
        Pages_Administration_VisaGuideManager = 22,

        [Description("Quote Manager")]
        Pages_Administration_QuoteManager = 23,

        #region [Action Permissions]
        [Description("Is ReadOnly")]
        Action_IsRead = 1,
        [Description("Is Create")]
        Action_IsCreate = 2,
        [Description("Is Edit")]
        Action_IsEdit = 3,
        [Description("Is Delete")]
        Action_IsDelete = 4,
        #endregion [Action Permissions]

        //[Description("Admin Users")]
        //Pages_Administration_AdminUsers = 2,
        //[Description("List")]
        //Pages_Administration_AdminUsers_List = 3,
        //[Description("Create")]
        //Pages_Administration_AdminUsers_Create = 4,
        //[Description("Edit")]
        //Pages_Administration_AdminUsers_Edit = 5,
        //[Description("Delete")]
        //Pages_Administration_AdminUsers_Delete = 6,

        //[Description("Roles")]
        //Pages_Administration_Roles = 7,
        //[Description("List")]
        //Pages_Administration_Roles_List = 8,
        //[Description("Create")]
        //Pages_Administration_Roles_Create = 9,
        //[Description("Edit")]
        //Pages_Administration_Roles_Edit = 10,
        //[Description("Delete")]
        //Pages_Administration_Roles_Delete = 11,


    }
    public enum StaticRole : int
    {
        [Description("SuperAdmin")]
        Administrator = 1
    }

}
