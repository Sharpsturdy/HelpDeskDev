﻿using Hangfire.Dashboard;
using Microsoft.Owin;
using System.Collections.Generic;

namespace Help_Desk_2.Utilities
{
    public class HangFireAuthorize : IAuthorizationFilter
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return context.Authentication.User.Identity.IsAuthenticated;
        }
    }

}