using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.EventManager.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /EventManager/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
