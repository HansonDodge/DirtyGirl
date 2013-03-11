using System.Diagnostics;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Utils;
using FacebookOpenGraph.Authentication;
using FacebookOpenGraph.Graph;
using FacebookOpenGraph.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DirtyGirl.Web.Controllers
{
    [Authorize(Roles = "Registrant")]
    public class UserController : BaseController
    {

        #region Constructor

        public UserController(IUserService userService)
        {
            UserService = userService;            
        }

        #endregion

        #region Create User
        
        [AllowAnonymous]
        public ActionResult CreateUser(string returnUrl)
        {
            var vm = new vmUser_EditUser
                {
                    returnUrl = returnUrl
                };

            Debug.Assert(TempData != null, "TempData != null");
            var user = TempData["FacebookUser"] as FacebookUser;
            if (user != null)
            {
                var facebookUser = user;
                vm.User.FirstName = facebookUser.FirstName;
                vm.User.LastName = facebookUser.LastName;
                vm.User.EmailAddress = facebookUser.Email;
                vm.EmailAddressVerification = facebookUser.Email;
                vm.User.FacebookId = int.Parse(facebookUser.Id);
                vm.User.UseFacebookImage = true;                
                DisplayMessageToUser(new DisplayMessage(DisplayMessageType.General,
                    "Thank you for logging in in with Facebook, please provide some additional information to complete your Dirty Girl account registration.”."));
            }

            FillEditUserEnums(vm);

            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CreateUser(vmUser_EditUser vm)
        {
            //bool validImageFile = true;
            if (!vm.ImAGirl)
                ModelState.AddModelError("ImAGirl", "You must confirm you are a female.");
            if(vm.User.UserName.Length < 3)
                ModelState.AddModelError("UserName", "Usernames must be 3 or more characters long.");

            if (ModelState.IsValid)
            {
                if (vm.Image != null)
                {
                    MemoryStream m = new MemoryStream();
                    vm.Image.InputStream.CopyTo(m);
                    vm.User.Image = m.ToArray();
                }

                ServiceResult result = UserService.CreateUser(vm.User);
                /*
                if (!validImageFile)
                    result.AddServiceError("Images must be .jpg, .png, .gif, and less than 2 megabytes in size");
                */
                

                if (result.Success)
                {                
                    FormsAuthentication.SetAuthCookie(vm.User.UserName, false);

                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "We've setup your profile and you're ready to register for your first race."));

                    if (!string.IsNullOrEmpty(vm.returnUrl))
                        return Redirect(vm.returnUrl);
                            
                    return RedirectToAction("ViewUser", new {userId = vm.User.UserId});
                }
                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            }      

            FillEditUserEnums(vm);

            return View(vm);
        }

        #endregion

        #region View User

        [HttpGet]
        public ActionResult ViewUser(int userId)
        {
            OnlyOwnerAccess(userId);
            var vm = new vmUser_ViewUser {
                User = UserService.GetUserById(userId),
                Registrations = UserService.GetActiveRegistrations(CurrentUser.UserId),
                RegistrationValues = new Dictionary<int,decimal>(),
                OpenCodes = UserService.GetActiveRedemptionCodes(CurrentUser.UserId)
            };

            foreach (var reg in vm.Registrations)
            {
                vm.RegistrationValues.Add(reg.RegistrationId, UserService.GetRegistrationValue(reg.RegistrationId));
            }
            
            vm.Registrations = vm.User.Registrations.Where(x => x.EventWave.StartTime > DateTime.Now && x.RegistrationStatus == RegistrationStatus.Active).OrderBy(x => x.EventWave.StartTime).ToList();
            return View(vm);
        }

        #endregion

        #region Edit User

        [HttpGet]
        public ActionResult EditUser(int userId, string returnUrl = "")
        {
            OnlyOwnerAccess(userId);

            var vm = new vmUser_EditUser { User = UserService.GetUserById(userId) };            
            vm.EmailAddressVerification = vm.EmailAddress;
            vm.returnUrl = returnUrl; 
            
            FillEditUserEnums(vm);

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditUser(vmUser_EditUser vm)
        {
            //bool validImageFile = true;

            OnlyOwnerAccess(vm.User.UserId);
            
            if (ModelState.IsValid)
            {
                /*
                if (image != null && image.ContentLength > 0 && image.ContentLength <= 2048000)
                {
                    var target = new MemoryStream();
                    image.InputStream.CopyTo(target);
                    if (Utilities.VerifyFileIsImage(target))
                        vm.User.Image = target.ToArray();
                    else
                        validImageFile = false;
                }
                */
                ServiceResult result =  UserService.UpdateUser(vm.User, false);
                /*
                if (!validImageFile)
                    result.AddServiceError("Images must be .jpg, .png, .gif, and less than 2 megabytes in size");
                */

                if (result.Success)
                {
                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "User profile has been updated successfully"));
                    if (!string.IsNullOrEmpty(vm.returnUrl)){
                        Response.Redirect(vm.returnUrl); 
                    } else {
                        return RedirectToAction("ViewUser", new {userId = vm.User.UserId});
                    }
                    
                }
                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            }

            FillEditUserEnums(vm);

            return View(vm);
        }

        #endregion

        #region Change Password
        
        [HttpGet]
        public ActionResult ChangePassword(int userId)
        {
            OnlyOwnerAccess(userId);
            User user = UserService.GetUserById(userId); 
            var vm = new vmUser_ChangePassword
                         {
                             Credentials =
                                 {
                                     UserId = user.UserId,
                                     Username = user.UserName
                                 }
                         };
            return View(vm);
        }

        [HttpPost]
        public ActionResult ChangePassword(vmUser_ChangePassword vm)
        {
            OnlyOwnerAccess(vm.Credentials.UserId);
            if (ModelState.IsValid)
            {
                if (!UserService.ValidateUser(vm.Credentials.UserId,vm.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password Incorrect");
                }
                else
                {
                    ServiceResult result = UserService.UpdatePassword(vm.Credentials.UserId, vm.Credentials.NewPassword);
                    if (result.Success)
                    {
                        DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage,
                                                                "Password has been updated successfully"));
                        return RedirectToAction("EditUser", new {userId = vm.Credentials.UserId});
                    }
                    Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
                }
            }
            return View(vm);

        }

        #endregion 

        #region UserImage

        [HttpGet]
        public ActionResult UserImage(int userId)
        {
            //TODO: if(User.IsInRole("Administrator") || User.Identity.Name.Equals(id.ToString()))
            var imageData = UserService.GetUserById(userId).Image;
            return imageData != null ? File(imageData, "image/png") : null;
        }

        #endregion

        #region Facebook

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Facebook(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                if (!string.IsNullOrEmpty(Request.QueryString["error"])) //User hit cancel in auth dialog
                    return null;
                if (state != null)
                {
                    state = "&state=" + state;
                }
                return Redirect(FacebookAuthentication.GetFaceBookAuthUrl() + state);
            }
            //var dateAuthed = DateTime.Now;
            AccessToken token;

            try
            {
                token = FacebookAuthentication.GetFacebookAccessToken(code);
            }
            catch (FacebookException)
            {
                return null;
            }

            var fbUser = FacebookGraph.GetPublicData(token.Token);

            //If the user is already registered with the DG site they'll be redirected
            long fbId;
            bool result = long.TryParse(fbUser.Id, out fbId);

            if (result)
            {
                var dgUser = UserService.GetUserByFacebookId(fbId);

                if (dgUser != null)
                {
                    FormsAuthentication.SetAuthCookie(dgUser.UserName, false);
                    if (state != null)
                    {
                        return Redirect(state);
                    }
                    return RedirectToAction("ViewUser", new { userId = dgUser.UserId });
                }
                TempData["FacebookUser"] = fbUser;
            }

            return RedirectToAction("CreateUser", new {redirectUrl = state});
        }

        #endregion

        #region username availability

        [AllowAnonymous]
        [HttpGet]
        public int CheckUsernameAvailability(string username)
        {
            if (UserService.CheckUsernameAvailability(username)) { return 1; }
            return 0;
        }

        #endregion

        #region Confirmation Code

        [AllowAnonymous]
        [HttpGet]
        public ActionResult EnterConfirmationCode(string confirmationCode)
        {
            if (confirmationCode != null)
            {
                var result = UserService.ConfirmAccount(confirmationCode);

                if (result != null)
                {
                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "Account has been confirmed."));
                    return RedirectToAction("ViewUser", new { userId = result.Value});
                }
                Utilities.AddModelStateErrors(ModelState, new List<ServiceError> { new ServiceError("confirmationCode", "The confirmation code doesn't match any users. Please verify and re-enter the code or click the direct link in the email.") });
            }
            else
            {
                Utilities.AddModelStateErrors(ModelState, new List<ServiceError> { new ServiceError("confirmationCode", "You must enter a confirmation code.") });
            }

            return View();
        }

        [HttpGet]
        public ActionResult SendConfirmationEmail(int userId)
        {
            OnlyOwnerAccess(userId);

            DisplayMessageToUser(UserService.SendEmailConfirmation(userId)
                                     ? new DisplayMessage(DisplayMessageType.SuccessMessage, "Confirmation Email Sent")
                                     : new DisplayMessage(DisplayMessageType.ErrorMessage,"Error - Confirmation Email Not Sent"));

            return RedirectToAction("ViewUser", new { userId });
        }
        #endregion

        #region private methods

        private void FillEditUserEnums(vmUser_EditUser model)
        {
            model.Regions = UserService.GetRegionsForCountry(DirtyGirlConfig.Settings.DefaultCountryId);

            model.Months = DirtyGirlExtensions.ConvertToSelectList<Months>();

            model.Days = new List<int>();
            for (int i = 1; i <= 31; i++)
            {
                model.Days.Add(i);
            }

            model.Years = new List<int>();
            for (int i = DateTime.Now.Year; i >= DateTime.Now.Year - 100; i--)
            {
                model.Years.Add(i);
            }

        }

        #endregion

    }
}
