using System;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using System.Web.Mvc;
using System.Linq;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Web.Controllers
{
    
    public class ErrorController : Controller
    {

        #region Constructor       

      
        #endregion

        public ActionResult MudPlant()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }      

    }
}
