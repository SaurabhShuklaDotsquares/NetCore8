using Microsoft.AspNetCore.Mvc;
using TCP.Service;
using TCP.Web.ViewModels.VisaGuide;

namespace TCP.Web.Controllers
{
    public class VisaGuideController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IVisaGuideService _visaGuideService;
        private readonly ICountryService _countryService;

        public VisaGuideController(IUserService userService, IVisaGuideService visaGuideService, ICountryService countryService)
        {
            _userService = userService;
            _visaGuideService = visaGuideService;
            _countryService = countryService;
        }


        public IActionResult Index()
        {
            VisaGuideViewModel vm = new VisaGuideViewModel();
            string pageName = Convert.ToString(RouteData.Values["visa_url"]);
            if (pageName!=null)
            {
                var visaguidDtls = _visaGuideService.GetVisaGuidePageByUrl(Convert.ToString(RouteData.Values["visa_url"]));
               //var country_name= _countryService.GetCountryMasterById(Convert.ToInt32(visaguidDtls.CountryId)).Name;
                if (visaguidDtls != null)
                {
                    vm.Id = visaguidDtls.Id;
                    vm.PageTitle = visaguidDtls.PageTitle;
                    vm.ContentData = visaguidDtls.ContentData;
                    //vm.CountryName = visaguidDtls.;
                }
            }
            return View(vm);
        }
    }
}
