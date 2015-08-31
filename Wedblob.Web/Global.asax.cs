using Autofac;
using Forloop.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Wedblob.Web.Infrastructure.Startup;

namespace Wedblob.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            //create the IoC container and run all the startup tasks
            var container = new AutoFacConfig().BuildContainer();
            using(var scope = container.BeginLifetimeScope())
            {
                foreach (var startup in container
                    .Resolve<IEnumerable<IStartupTask>>()
                    .OrderBy(x => x.ExecutionOrder))
                    startup.Execute();
            }
        }
    }
}
