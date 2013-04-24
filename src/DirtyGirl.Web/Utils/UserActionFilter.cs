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

            // need to find if in admin or not
            var controllerType = filterContext.Controller.GetType();
            if (controllerType.FullName.Contains("Areas.Admin"))
            {
                filterContext.Controller.ViewData[BaseController.CurrentUserKey] = ((DirtyGirl.Web.Areas.Admin.Controllers.BaseController)filterContext.Controller).CurrentUser;
            }
            else if (controllerType.FullName.Contains("Areas.EventManager"))
            {
                filterContext.Controller.ViewData[BaseController.CurrentUserKey] = ((DirtyGirl.Web.Areas.EventManager.Controllers.BaseController)filterContext.Controller).CurrentUser;
            }
            else
            {
                filterContext.Controller.ViewData[BaseController.CurrentUserKey] = ((BaseController)filterContext.Controller).CurrentUser;
            }
        }
    }
}
