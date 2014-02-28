using System.Diagnostics;

namespace VMMonitoringWebApplication.Controllers
{
    using Elmah.Contrib.Mvc;
    using System.Web.Mvc;
    using Infastracture;
    using Models;

    public class HomeController : Controller
    {
        private readonly IRememberService rememberService;

        public HomeController(IRememberService rememberService)
        {
            this.rememberService = rememberService;
        }

        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            //if (this.rememberService.Check(DefaultNames.ServerCookieName))
            //{
            //    var list = this.rememberService.GetListFromCookies<ServerModel>(DefaultNames.ServerCookieName).ToList();

            //    return View(list);
            //}

            var model = this.rememberService.Get<ServerModel>(DefaultNames.ServerCookieName);

            return View(model);
        }

        public ActionResult List()
        {
            var model = this.rememberService.Get<ServerModel>(DefaultNames.ServerCookieName);

            return View(model);
        }

        public JsonResult JsonAddServer(ServerModel model, string returnUrl)
        {
            this.rememberService.Add(DefaultNames.ServerCookieName, model);

            return Json(new { success = true, redirect = Url.Action("List") });
        }

        public JsonResult JsonAddVM(VMItemDto model, string returnUrl)
        {
            this.rememberService.Add(DefaultNames.VMName, model);

            return Json(new { success = true, redirect = Url.Action("GetVMFromMemory", "Chart") });
        }
    }
}