using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(VMMonitoringWebApplication.Startup))]

namespace VMMonitoringWebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}