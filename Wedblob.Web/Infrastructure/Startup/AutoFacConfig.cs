using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wedblob.Web.Services;

namespace Wedblob.Web.Infrastructure.Startup
{
    public class AutoFacConfig
    {
        public IContainer BuildContainer()
        {
            var thisAssembly = typeof(AutoFacConfig).Assembly;

            var builder = new ContainerBuilder();
            builder.RegisterControllers(thisAssembly);
            builder.RegisterModelBinders(thisAssembly);
            builder.RegisterInstance(RouteTable.Routes).As<RouteCollection>();
            builder.RegisterInstance(GlobalFilters.Filters).As<GlobalFilterCollection>();


            builder.RegisterType<Settings>().As<ISettings>().SingleInstance();
            builder.RegisterType<ContentService>().As<IContentService>();
            builder.Register(c => new Database("db"));
             
            builder.RegisterAssemblyTypes(thisAssembly)
                .Where(t => t.IsAssignableTo<IStartupTask>())
                .As<IStartupTask>();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }
    }



}