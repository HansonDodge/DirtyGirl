using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Controllers
{
    public class DummyController : Controller
    {
        //
        // GET: /Dummy/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        public ActionResult Wave()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult Donate()
        {
            return View();
        }

        public ActionResult Summary()
        {
            return View();
        }

        public ActionResult Profile1()
        {
            return View("Profile");
        }

        public ActionResult TeamChat()
        {
            return View();
        }

        public ActionResult EditRegistration()
        {
            return View();
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}
