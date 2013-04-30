using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Utils;
using Elmah;
using Ninject;
using System;
using System.Web;
using System.Web.Mvc;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    [Authorize(Roles="admin, superadmin")]
    public abstract class BaseController : Controller
    {

        #region private members

        [Inject]
        public IUserService UserService { get; set; }

        private User _currentUser;

        #endregion

        #region public properties

        public const string CurrentUserKey = "_currentuser";

        public User CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = UserService.GetUserByUsername(HttpContext.User.Identity.Name);

                    // if no user return a empty user 
                    if (_currentUser == null) { return new User(); }
                }

                return _currentUser;
            }
        }

        #endregion

        #region protected methods

        protected void OnlyOwnerAccess(int userId)
        {
            if (CurrentUser == null || (!HttpContext.User.IsInRole("Admin") && userId != CurrentUser.UserId))
                throw new UnauthorizedAccessException("Users may only view or modify their own settings.");
        }

        protected void OnlyOwnerAccess(string userName)
        {
            if (userName == null || CurrentUser == null || (!HttpContext.User.IsInRole("Admin") && userName.ToLower() != CurrentUser.UserName.ToLower()))
                throw new UnauthorizedAccessException("Users may only view or modify their own settings.");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                Response.StatusCode = 500;
                Response.TrySkipIisCustomErrors = true;
                ForceElmahNotification(filterContext.Exception);
                switch (filterContext.Exception.GetType().Name)
                {
                    case "UnauthorizedAccessException":
                        View("AuthError").ExecuteResult(ControllerContext);
                        return;
                    default:
                        View("Error").ExecuteResult(ControllerContext);
                        return;
                }
            }
        }

        protected void DisplayMessageToUser(DisplayMessage message)
        {
            TempData[DirtyGirlConfig.Settings.DisplayMessageKey] = message;
        }

        #endregion

        protected static void ForceElmahNotification(Exception exception)
        {
            var context = System.Web.HttpContext.Current;
            if (context == null) return;
            var signal = ErrorSignal.FromContext(context);
            if (signal != null) signal.Raise(exception, context);
        }

    }
}
