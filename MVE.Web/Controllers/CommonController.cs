using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Principal;
using TCP.Core;
using TCP.Core.Code.LIBS;
using TCP.Core.Models;
using TCP.Data.Models;
using TCP.Service;
using TCP.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TCP.Web.Controllers
{
    public class CommonController : BaseController
    {
        private readonly IStaticService staticService;

        public CommonController(IStaticService staticService)
        {            
            this.staticService = staticService;           
        }

       
        [HttpGet]
        public IActionResult ViewSharePopup()
        {
            //OomamiUser oomamiUser = oomamiUserService.GetUserById(Convert.ToInt32(id));
            return PartialView("_ModalShare");
        }

        public IActionResult StaticPages()
        {
            string pageName = Convert.ToString(RouteData.Values["page_url"]);
            if (pageName != null)
            {
                var staticpageData = staticService.GetStaticPageByUrl(Convert.ToString(RouteData.Values["page_url"]));
                if (staticpageData != null)
                {
                    StaticViewModel staticPageViewModel = new StaticViewModel();
                    staticPageViewModel.Name = staticpageData.Name;
                    staticPageViewModel.Content = staticpageData.Content;
                    staticPageViewModel.MetaKeyword = staticpageData.MetaKeyword;
                    staticPageViewModel.MetaDescription = staticpageData.MetaDescription;
                    staticPageViewModel.Url = staticpageData.Url;
                    return View(staticPageViewModel);
                }

            }
            return View();
        }

    }
}
