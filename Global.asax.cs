using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Help_Desk_2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

		}

        protected void Application_BeginRequest()
        {
            var gbCultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-GB");
            Thread.CurrentThread.CurrentCulture = gbCultureInfo;
            Thread.CurrentThread.CurrentUICulture = gbCultureInfo;
        }

        

    }
}
