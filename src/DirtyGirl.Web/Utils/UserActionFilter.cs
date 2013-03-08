using System.IO;
using System.Web;
using System.Web.Mvc;
using DirtyGirl.Models;
using DirtyGirl.Services;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Controllers;

namespace DirtyGirl.Web.Utils
{
    public class UserActionFilter : ActionFilterAttribute
    {
        // stores CurrentUser in ViewData 
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewData[BaseController.CurrentUserKey] = ((BaseController)filterContext.Controller).CurrentUser;
        }
    }
}
