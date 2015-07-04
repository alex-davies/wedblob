using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using HashidsNet;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
            builder.RegisterType<RSVPService>().As<IRSVPService>();
            builder.RegisterGeneric(typeof(IdGenerator<>)).As(typeof(IIdGenerator<>));


            builder.Register(c => new MongoClient(c.Resolve<ISettings>().MongoDBConnectionString)).As<IMongoClient>().SingleInstance();
            builder.Register(c => c.Resolve<IMongoClient>().GetDatabase(c.Resolve<ISettings>().MongoDBName)).As<IMongoDatabase>().SingleInstance();
            builder.Register(c => new Hashids(c.Resolve<ISettings>().HashIdSalt, minHashLength: c.Resolve<ISettings>().HashIdMinLength, alphabet:c.Resolve<ISettings>().HashIdAlphabet)).As<Hashids>().SingleInstance();

             
            builder.RegisterAssemblyTypes(thisAssembly)
                .Where(t => t.IsAssignableTo<IStartupTask>())
                .As<IStartupTask>();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            return container;
        }
    }



}