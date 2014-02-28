namespace VMMonitoringWebApplication
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.SignalR;
    using Autofac.Integration.WebApi;
    using Microsoft.AspNet.SignalR;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;
    using Infastracture;
    using System.Web.Configuration;

    public static class ContainerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            string domainUser = WebConfigurationManager.AppSettings["domainUser"];
            string password = WebConfigurationManager.AppSettings["pwd"];
            string vmmServerName = WebConfigurationManager.AppSettings["vmmserver"];

            string connectionString = WebConfigurationManager.ConnectionStrings["VMMDBConnection"].ConnectionString;

            builder.Register(p => new PowerShellProvider(domainUser, password, vmmServerName)).As<IProvider<PSCommand>>().InstancePerLifetimeScope();
            builder.Register(p => new DapperProvider(connectionString)).As<IProvider<DapperCommand>>().InstancePerLifetimeScope();
            

#if DEBUG
            builder.RegisterType<TestMonitoringService>().As<IMonitoringService>().SingleInstance();
            builder.RegisterType<TestHostService>().As<IHostService>().InstancePerApiRequest();
            //builder.RegisterType<HostService>().As<IHostService>().InstancePerApiRequest();
            //builder.RegisterType<SPMonitoringService>().As<IMonitoringService>().SingleInstance();

            //builder.RegisterType<MonitoringService>().As<IMonitoringService>().SingleInstance();
#else
            builder.RegisterType<HostService>().As<IHostService>().OnActivating(e => e.Instance.SetServer(vmmServerName)).InstancePerApiRequest();

            builder.RegisterType<SPMonitoringService>().As<IMonitoringService>().SingleInstance();
#endif
            builder.RegisterType<DefaultRememberService>().As<IRememberService>().SingleInstance();
            
            builder.RegisterFilterProvider();

            var container = builder.Build();
            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}