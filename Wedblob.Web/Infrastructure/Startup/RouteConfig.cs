using Wedblob.Web.Infrastructure.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wedblob.Web.Infrastructure.Startup
{
    public class RouteConfig : IStartupTask
    {
        public readonly RouteCollection Routes;

        public RouteConfig(RouteCollection routes)
        {
            this.Routes = routes;
        }

        public void Execute()
        {
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Default", action = "Index" }
            );
        }

        public int ExecutionOrder
        {
            get { return 0; }
        }
    }
}
