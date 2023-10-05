using Hangfire.Dashboard;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
#pragma warning disable CS0618 // 'IAuthorizationFilter' is obsolete: 'Please use `IDashboardAuthorizationFilter` instead. Will be removed in 2.0.0.'
    public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
#pragma warning restore CS0618 // 'IAuthorizationFilter' is obsolete: 'Please use `IDashboardAuthorizationFilter` instead. Will be removed in 2.0.0.'
    {
        public bool Authorize(IDictionary<string, object> owinEnvironment)
        {
            // In case you need an OWIN context, use the next line,
            // `OwinContext` class is the part of the `Microsoft.Owin` package.
            var context = new OwinContext(owinEnvironment);

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return (context.Authentication.User.Identity.IsAuthenticated && 
                (context.Authentication.User.Identity.Name == "admin" || context.Authentication.User.Identity.Name == "superadmin"));
        }
    }
}