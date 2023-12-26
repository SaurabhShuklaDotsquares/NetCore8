using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing.Printing;
using MVE.Admin.ViewModels;
using MVE.Core;
using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.DataTable.Extension;
using MVE.DataTable.Search;
using MVE.DataTable.Sort;
using MVE.Service;


namespace MVE.Admin.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment WebHostEnvironment;
        private readonly IUserService _userService;
        private readonly IThemeService _themeService;
        private readonly ICountryService _countryService;
        public DashboardController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IUserService userService, IThemeService themeService, ICountryService countryService)
        {
            _configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
            _userService = userService;
            _themeService = themeService;
            _countryService = countryService;
        }

        public IActionResult Index()
        {
            DashboardViewModel vm = new DashboardViewModel();
            vm.SetEntity(CurrentUser);

            vm.SetHostingEnvironment(WebHostEnvironment);

            //Users
            var users = _userService.GetAllUserNotDeleted();
            if (users != null && users.Count > 0)
            {
                vm.TotalUser = users.Count;
            }
            //Category
            var themes = _themeService.GetAllThemes(true, false).ToList();
            if (themes != null && themes.Count > 0)
            {
                vm.TotalActiveCategories = themes.Count;
            }

            //Booking
            var query = new SearchQuery<Booking>();
            //query.AddFilter(q => q.BookingStatusId == (int)BookingStatus.Confirmed);
            int total = 0;

            vm.GTotalResolvedEnquiries = 0;

            return View(vm);
        }
        [HttpGet]
        public IActionResult GetChartData(string type)
        {
            DateTime fromDate = DateTime.Now;
            var usersdata = _userService.GetAllUserNotDeleted();
           
            return NewtonSoftJsonResult(new RequestOutcome<object> { Data = null, Message = "Success" }); ;
        }
        [HttpGet]
        public IActionResult GetEarningChartData(string type)
        {
            DateTime fromDate = DateTime.Now;

           

            return NewtonSoftJsonResult(new RequestOutcome<object> { Data = null, Message = "Success" }); ;
        }
        [HttpPost]
        public ActionResult GetCountrieswisesaleslist(MVE.DataTable.DataTables.DataTable dataTable)
        {

            List<DataTableRow> table = new List<DataTableRow>();

            int count = 1, total = 0;

            return new DataTableResultExt(dataTable, table.Count(), total, table);
        }
    }
}
