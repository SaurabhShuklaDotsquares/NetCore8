using Microsoft.AspNetCore.Mvc.Razor;
using TCP.Core.Models.Security;

namespace TCP.Web.Models
{
    public abstract class BaseViewPage<TModel> : RazorPage<TModel>
    {
        protected CustomPrincipal CurrentFrontUser => new CustomPrincipal(ContextProvider.HttpContext.User);

        protected object getHtmlAttributes(bool readonl, string cssClass)
        {
            if (readonl)
            {
                return new { @class = cssClass, @readonly = true };
            }
            return new { @class = cssClass };
        }
    }
}
