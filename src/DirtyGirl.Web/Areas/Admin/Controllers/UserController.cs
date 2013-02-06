using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Security;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Utils;
using System.Web;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using DirtyGirl.Web.Models;
using DirtyGirl.Web.Controllers;
using System.IO;

namespace DirtyGirl.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        #region private members

        #endregion

        #region Constructor

        public UserController(IUserService userService)
        {
            UserService = userService;
        }

        #endregion

        [HttpGet]
        public ActionResult ViewUser(int id = 0, string username = "")
        {
            vmUser_EditUser vm;
            if (id > 0)
            {
                OnlyOwnerAccess(id);
                vm = new vmUser_EditUser {User = UserService.GetUserById(id)};
            }
            else
            {
                OnlyOwnerAccess(username);
                vm = new vmUser_EditUser { User = UserService.GetUserByUsername(username) };
            }
            FillEditUserEnums(ref vm);
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditUser(int? id)
        {

            var vm = new vmUser_EditUser();

            if (id.HasValue)
            {
                OnlyOwnerAccess(id.Value);
                vm.User = UserService.GetUserById(id.Value);
                vm.EmailAddressVerification = vm.User.EmailAddress;
                vm.AllExistingRoles = Roles.GetAllRoles();
                vm.UsersRoles = Roles.GetRolesForUser();
            }
            if (vm.User == null)
            {
                vm.User = new User();
            }

            FillEditUserEnums(ref vm);

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditUser(vmUser_EditUser vm, HttpPostedFileBase image)
        {

            if (ModelState.IsValid)
            {
              bool ValidImageFile = true;
                OnlyOwnerAccess(vm.User.UserId);

                if (image != null && image.ContentLength > 0 && image.ContentLength <= 2048000 &&
                        (image.FileName.EndsWith(".jpg")
                        || image.FileName.EndsWith(".bmp")
                        || image.FileName.EndsWith(".png")
                        || image.FileName.EndsWith(".gif")
                        || image.FileName.EndsWith(".tiff"))
                    )
                {
                  var target = new MemoryStream();
                  image.InputStream.CopyTo(target);
                  vm.User.Image = target.ToArray();
                }
                else
                {
                  if (image != null && image.ContentLength > 0)
                    ValidImageFile = false;
                }

                var result = vm.User.UserId <= 0 ? UserService.CreateUser(vm.User) : UserService.UpdateUser(vm.User, true);

                if (!ValidImageFile)
                  result.AddServiceError("Images must be .jpg, .bmp, .png, .gif, .tiff and < 2MB in size");

                if (result.Success)
                {
                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "User has been saved successfully"));
                    return RedirectToAction("ViewUser", new { id = vm.User.UserId });
                }

                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            }
            FillEditUserEnums(ref vm);

            return View(vm);
        }

        [HttpGet]
        public ActionResult ListUsers()
        {
            return View(new vmUser_UserList());//new vmUser_UserList(_userService.GetAllUsers()));
        }

        [HttpGet]
        public ActionResult SetPassword(int id)
        {
            var sp = new vmUser_SetPassword {UserId = id, Username = UserService.GetUserById(id).UserName};
            return View(sp);
        }

        [HttpPost]
        public ActionResult SetPassword(vmUser_SetPassword sp)
        {
            if (ModelState.IsValid)
            {
                ServiceResult result = UserService.UpdatePassword(sp.UserId, sp.NewPassword);
                if (result.Success)
                {
                    DisplayMessageToUser(new DisplayMessage(DisplayMessageType.SuccessMessage, "Password has been updated successfully"));
                    return RedirectToAction("Edituser", new { id = sp.UserId });
                }
                Utilities.AddModelStateErrors(ModelState, result.GetServiceErrors());
            }
            return View(sp);
        }

        [HttpGet]
        public ActionResult UserImage(int id)
        {
            var imageData = UserService.GetUserById(id).Image;
            if (imageData != null)
            {
                return File(imageData, "image/png");
            }
            return null;
        }



        #region Private Methods

        private void FillEditUserEnums(ref vmUser_EditUser model)
        {
            model.Regions = UserService.GetRegionsForCountry(DirtyGirlConfig.Settings.DefaultCountryId);
        }

        #endregion

        public ActionResult SearchUsers([DataSourceRequest]DataSourceRequest request, string name = null, string userNameEmail = null)
        {

            IEnumerable<User> userList = null;
            if (name != null) name = name.ToLowerInvariant();
            if (userNameEmail != null) userNameEmail = userNameEmail.ToLowerInvariant();
            if (name != null && userNameEmail != null)
            {
                var namelist = name.ToLowerInvariant().Trim().Split(' ');
                if (namelist.Count() > 1)
                {
                    userList =
                        UserService.GetAllUsers().Where(
                            user => (namelist.Any(array => user.FirstName.ToLowerInvariant().Contains(array))
                                     && namelist.Any(array => user.LastName.ToLowerInvariant().Contains(array)))
                                    && (user.UserName.ToLowerInvariant().Contains(userNameEmail)
                                        || user.EmailAddress.ToLowerInvariant().Contains(userNameEmail)));
                }
                else
                {
                    userList =
                        UserService.GetAllUsers().Where(
                            user => (namelist.Any(array => user.FirstName.ToLowerInvariant().Contains(array))
                                     || namelist.Any(array => user.LastName.ToLowerInvariant().Contains(array)))
                                    && (user.UserName.ToLowerInvariant().Contains(userNameEmail)
                                        || user.EmailAddress.ToLowerInvariant().Contains(userNameEmail)));
                }
            }

            Debug.Assert(userList != null, "userList != null");
            var returnUserList = userList.Select(user => new vmUser_Detail
                                                             {
                                                                 UserId = user.UserId,
                                                                 FirstName = user.FirstName,
                                                                 LastName = user.LastName,
                                                                 UserName = user.UserName,
                                                                 IsActive = user.IsActive,
                                                                 Locality = user.Locality
                                                             }).ToList();
            var result = returnUserList.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchUsersTypeAhead(string name)
        {
            var userList = UserService.GetAllUsers().Where(user => user.UserName.Contains(name) || user.FirstName.Contains(name) || user.LastName.Contains(name));
            var returnUserList = userList.Select(user => new vmUser_Detail
            {
                UserId = user.UserId
                ,
                FirstName = user.FirstName
                ,
                LastName = user.LastName
                ,
                UserName = user.UserName
                ,
                IsActive = user.IsActive
                ,
                Locality = user.Locality
            }).ToList();
            return Json(returnUserList, JsonRequestBehavior.AllowGet);
        }

    }
}
