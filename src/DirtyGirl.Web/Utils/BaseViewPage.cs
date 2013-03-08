using System.IO;
using System.Web;
using System.Web.Mvc;
using DirtyGirl.Models;
using DirtyGirl.Services;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Controllers;

namespace DirtyGirl.Web.Utils
{
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {

        protected User CurrentUser
        {
            get
            {
                return ViewData[BaseController.CurrentUserKey] as User;
            }
        }
    }
}
