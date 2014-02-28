namespace VMMonitoringWebApplication.Controllers
{
    using Elmah.Contrib.Mvc;
    using System.Web.Mvc;
    using Infastracture;
    using Models;

    public class ChartController : Controller
    {
        private readonly IRememberService rememberService;

        public ChartController(IRememberService rememberService)
        {
            this.rememberService = rememberService;
        }

        public ActionResult Index(VMItemDto model)
        {
            return View(model);
        }

        public ActionResult GetVMFromMemory(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            var model = rememberService.Get<VMItemDto>(DefaultNames.VMName);

            return RedirectToAction("Index", model);
        }

        public ActionResult GetVMByName(string id)
        {
            var model = new VMItemDto { Name = id };

            return RedirectToAction("Index", model);
        }
    }
}
