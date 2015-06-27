using Wedblob.Web.Infrastructure.Startup;
using System.Web;
using System.Web.Mvc;

namespace Wedblob.Web.Infrastructure.Startup
{
    public class FilterConfig : IStartupTask
    {
        public readonly GlobalFilterCollection Filters;
        
        public FilterConfig(GlobalFilterCollection filters)
        {
            this.Filters = filters;   
        }

        public void Execute()
        {
            Filters.Add(new HandleErrorAttribute());
        }

        public int ExecutionOrder
        {
            get { return 0; }
        }
    }
}
