using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DirtyGirl.Services.ServiceInterfaces;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    public class UpgradeController : BaseController
    {
        #region Private Members

        private readonly IUserService _userService;

        #endregion

        #region Constructor

        public UpgradeController(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PasswordReset()
        {

            try
            {
                var users = _userService.GetAllUsers();

                int count = 0;
                foreach (var user in users)
                {
                    Console.WriteLine(string.Format("User {0}", user.EmailAddress));
                    _userService.GeneratePasswordResetRequestForGoLive(user);
                    count++;
                }
                TempData["Errors"] = "Records Processed: " + count.ToString();
            }
            catch (Exception ex)
            {
                ForceElmahNotification(ex);
                TempData["Errors"] = ex.Message;                
            }
            return RedirectToAction("Index");
        }
    }
}
