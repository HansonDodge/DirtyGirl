using System.Web.Hosting;
using System.Web.Mvc;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;

namespace DirtyGirl.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ShareController : BaseController
    {
        private readonly IEmailService _emailService;

        public ShareController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Used to send sharing email.
        /// </summary>
        /// <param name="messageBody"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetShareEmail(string messageBody)
        {
                var shareModel = new vmCommon_Share() {MessageBody = messageBody};
                return View(shareModel);
        }

        /// <summary>
        /// Used to send sharing email.
        /// </summary>
        /// <param name="shareModel">The share model.</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public string ShareEmail(vmCommon_Share shareModel)
        {
            if (ModelState.IsValid)
            {
                var serv = _emailService.SendTeamShareEmail(shareModel.EmailAddresses.Split(new[] {';', ','}),
                                                            shareModel.MessageSubject, shareModel.MessageBody.Replace("{CustomMessage}", shareModel.UserMessageBody));
                return serv ? "Success" : "Share Failed";
            }
            return "All required fields must be completed before sending";
        }
    }
}
