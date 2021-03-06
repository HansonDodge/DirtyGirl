﻿using System;
using System.Web;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Web.Utils;
using Elmah;
using Ninject;
using System.Web.Mvc;
using System.Web.Routing;

namespace DirtyGirl.Web.Controllers
{
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
            get {                
                if (_currentUser == null) {
                    _currentUser = UserService.GetUserByUsername(HttpContext.User.Identity.Name);
                    
                    // if no user return a empty user 
                    if (_currentUser == null) { return new User(); }    
                }
                
                return _currentUser;  
            }
        }

        #endregion

        #region constructor
        
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

                // special handling for HttpRequestValidationException because Elmah does not handle it
                if (filterContext.Exception is HttpRequestValidationException)
                {
                    Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current).Log(new Error(filterContext.Exception));
                  
                    //Let the request know what went wrong
                    filterContext.Controller.TempData["Exception"] = filterContext.Exception;

                    //redirect to error handler
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                        new { controller = "Error", action = "InvalidInputError" }));

                    // Stop any other exception handlers from running
                    filterContext.ExceptionHandled = true;

                    // CLear out anything already in the response
                    filterContext.HttpContext.Response.Clear();                                      
                    return;
                }
                
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

        private static void ForceElmahNotification(Exception exception)
        {
            var context = System.Web.HttpContext.Current; 
            if (context == null) return; 
            var signal = ErrorSignal.FromContext(context); 
            if (signal != null) signal.Raise(exception, context);
        }
}}
