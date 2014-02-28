namespace VMMonitoringWebApplication
{
    using Elmah.Contrib.Mvc;
    using Elmah.Contrib.WebApi;
    using System.Web.Mvc;
    using System.Web.Http.Filters;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new ElmahHandleErrorAttribute());
        }

        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new ElmahHandleErrorApiAttribute());
        }
    }
}