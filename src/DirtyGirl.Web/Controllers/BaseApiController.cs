using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Utils;
using Ninject;

namespace DirtyGirl.Web.Controllers
{
    public class BaseApiController : ApiController
    {

        #region private members

        [Inject]
        public IUserService UserService { get; set; }

        private User _currentUser;

        #endregion

        #region public properties

        public User CurrentUser
        {
            get { return _currentUser ?? (_currentUser = UserService.GetUserByUsername(HttpContext.Current.User.Identity.Name)); }
        }

        #endregion

        #region constructor

        public BaseApiController()
        {
            
        }

        #endregion

        //#region protected methods

        //protected void DisplayMessageToUser(DisplayMessage message)
        //{
        //    TempData[DirtyGirlConfig.Settings.DisplayMessageKey] = message;
        //}

        //#endregion

    }
}
