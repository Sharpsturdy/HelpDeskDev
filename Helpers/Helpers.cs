using Help_Desk_2.DataAccessLayer;
using Help_Desk_2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Help_Desk_2.Helpers;


namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtentions
    {
		/*public static bool UserCan11(this HttpContextBase context, Actions requiredAction)
        {
            if (context.Request.IsAuthenticated)
            {
                var user = (User)context.Session[SessionKey.LoggedInUser];

                UserService service = new UserService();

                return service.UserCan(user.Id, (int)requiredAction);
            }

            return false;
        }
        */


		public static MvcHtmlString SecureActionLinkNEW(this HtmlHelper htmlHelper, string linkText, string actionName,  string controllerName, string requiredRoles)
		{
			var user         = htmlHelper.ViewContext.HttpContext.User;
			bool isAdmin     = user.CustomIsInRole(UserRoles.DomainAdminRole);
			bool isSuperUser = user.CustomIsInRole(UserRoles.DomainSuperUserRole);
			if (isAdmin || isSuperUser)
			{
				return htmlHelper.ActionLink(linkText, actionName, controllerName);
			}
			return new MvcHtmlString(string.Empty);
		}

		public static MvcHtmlString SecureActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, string requiredAction)
        {
            //if (!htmlHelper.ViewContext.HttpContext.UserCan(requiredAction))
            if (!AllSorts.UserCan(requiredAction))
               return new MvcHtmlString(string.Empty);

            return htmlHelper.ActionLink(linkText, actionName, controllerName);
        }
    }
}