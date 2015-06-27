using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedblob.Web.Infrastructure.Startup
{
    interface IStartupTask
    {
        void Execute();
        
        int ExecutionOrder { get; }
    }
}
