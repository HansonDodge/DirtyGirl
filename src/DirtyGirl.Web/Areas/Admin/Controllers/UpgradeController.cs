using System;
using System.Globalization;
using System.Linq;
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
        public ActionResult PasswordReset( string emailCount)
        {

            try
            {
                int emailsToSend;
                if (!int.TryParse(emailCount, out emailsToSend))
                {
                    TempData["Errors"] = "Invalid Email Count";
                    return RedirectToAction("Index");
                }
                var users = _userService.GetAllUsers();

                int count = 0;
                foreach (var user in users.Where(x=>string.IsNullOrEmpty(x.PasswordResetToken)).Take(emailsToSend))
                {
                    Console.WriteLine(string.Format("User {0}", user.EmailAddress));
                    _userService.GeneratePasswordResetRequestForGoLive(user);
                    count++;
                }
                TempData["Errors"] = "Records Processed: " + count.ToString(CultureInfo.InvariantCulture);
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
