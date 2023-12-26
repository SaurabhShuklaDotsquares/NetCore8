using Microsoft.AspNetCore.Mvc;

namespace MVE.Admin.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult AccessDeniedAjax()
        {
            return PartialView("_AccessDenied");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
