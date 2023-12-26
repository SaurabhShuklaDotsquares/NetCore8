using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Models.Security;
using MVE.Core.Code.LIBS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using FluentAssertions.Common;
using Microsoft.Extensions.Logging;
using System.Net;



namespace MVE.Core.Code.Attributes
{
    public class CustomAuthorization : Attribute, IAuthorizationFilter
    {
        private object[] roleTypes;
        private int[] permission;
        private int permissionId;
        private int actionId;
      

        public CustomAuthorization(params AppPermissions[] permission)
        {
            this.permission = Array.ConvertAll(permission, value => (int)value);
        }

        protected virtual CustomPrincipal CurrentUser
        {
            get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            string returnUrl = $"{ContextProvider.AbsoluteUri.PathAndQuery}";
            var ID = returnUrl.Split('/').Last();
            var isNumeric = int.TryParse(ID, out int n);

            if (CurrentUser != null && CurrentUser.IsAuthenticated)
            {
                if (permission.Length > 2)
                {
                    if (isNumeric)
                    {
                        int lastIndex = permission[permission.Length - 2];
                        permissionId = lastIndex;
                        actionId = permission[permission.Length - 1];
                    }
                    else
                    {
                        permissionId = permission[0];
                        actionId = permission[1];
                    }
                }
                else
                {
                    permissionId = permission[0];
                    actionId = permission[1];
                }
                if (CurrentUser.Role != StaticRole.Administrator.GetEnumDescription())
                {

                    //if (!CurrentUser.HasPermission(permissionId))
                    //{
                    //    ReturnAccessDenied(filterContext);
                    //}
                    if (CurrentUser.HasPermission(permissionId))
                    {
                        if (!CheckActionPermission(permissionId, actionId))
                        {
                            ReturnAccessDenied(filterContext);
                        }
                    }
                    else
                    {
                        ReturnAccessDenied(filterContext);
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/Index");
            }

        }

        private bool CheckActionPermission(int permissionId, int actionId)
        {
            bool isPermit = false;
            var pagePermissions = CurrentUser.allActionPagePermissionList.Where(x => x.PageId == permissionId).FirstOrDefault();
            if (pagePermissions != null)
            {
                if (actionId == 1)
                    isPermit = pagePermissions.IsReadOnly;
                else if (actionId == 2)
                    isPermit = pagePermissions.IsCreate;
                else if (actionId == 3)
                    isPermit = pagePermissions.IsEdit;
                else if (actionId == 4)
                    isPermit = pagePermissions.IsDelete;
            }
            return isPermit;
        }

        private void ReturnAccessDenied(AuthorizationFilterContext filterContext)
        {
            var isAjax = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";
            if (isAjax)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "error",
                    action = "accessDeniedAjax"
                }));
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "error",
                    action = "accessDenied"
                }));
            }
        }


        public class CustomActionAuthorization : ActionFilterAttribute
        {
            protected virtual CustomPrincipal CurrentUser
            {
                get { return new CustomPrincipal(ContextProvider.HttpContext.User); }
            }
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
               
                if (filterContext != null)
                {
                    if (!CurrentUser.IsAuthenticated)
                    {
                        filterContext.Result = new RedirectResult("~/account/index");
                    }
                }
            }
        }

       
    }
}
