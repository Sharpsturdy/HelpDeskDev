using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Help_Desk_2.Utilities
{
    public class CustomAuthorise : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult(); // Try this but i'm not sure
            filterContext.Result = new RedirectResult("~/Home/Unauthorized");
        }

        /*
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }
        */
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false; var roles = GetAuthorizedRoles();

            var provider = new WindowsTokenRoleProvider();
            if (roles.Any(role => provider.IsUserInRole(httpContext.User.Identity.Name, role))) {
                return true;
            } return base.AuthorizeCore(httpContext);
        }

        private IEnumerable<string> GetAuthorizedRoles() {
            var appSettings = ConfigurationManager.AppSettings[Roles];
            if (string.IsNullOrEmpty(appSettings)) {
                Trace.TraceError("Missing AD groups in Web.config for Roles {0}", Roles);
                return new[] { "" };
            }
            return appSettings.Split(',');
        }

    }

    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class myAuthorizeAttribute : AuthorizeAttribute
    {

        //Custom named parameters for annotation
        public string ResourceKey { get; set; }
        public string OperationKey { get; set; }

        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }
            //User is logged in but has no access
            else {
                filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Account", action = "NotAuthorized" })
                );
            }
        }

        //Core authentication, called before each action
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //var b = myMembership.Instance.Member().IsLoggedIn;
            var b = true;
            //Is user logged in?
            if (b)
                //If user is logged in and we need a custom check:
                if (ResourceKey != null && OperationKey != null)
                    return true;
                   // return ecMembership.Instance.Member().ActivePermissions.Where(x => x.operation == OperationKey && x.resource == ResourceKey).Count() > 0;
            //Returns true or false, meaning allow or deny. False will call HandleUnauthorizedRequest above
            return b;
        }
    }

   
}