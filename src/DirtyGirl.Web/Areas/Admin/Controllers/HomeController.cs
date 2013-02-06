using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    [Authorize(Roles="Admin")]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
