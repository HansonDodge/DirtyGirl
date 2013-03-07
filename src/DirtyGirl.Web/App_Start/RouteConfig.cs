using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace DirtyGirl.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
                );

            routes.MapRoute(
                name: "RegistionConfirmationEmailLink",
                url: "User/EnterConfirmationCode/{ConfirmationCode}",
                defaults: new { controller = "User", action = "EnterConfirmationCode" },
                namespaces: new string[]{"DirtyGirl.Web.Controllers"}
                );

            routes.MapRoute(
                name: "UserController",
                url: "User/{action}/{username}",
                defaults: new { controller = "User" },
                namespaces: new string[] { "DirtyGirl.Web.Controllers" }
                );


            routes.MapRoute(
               name: "MudPlant",
               url: "MudPlant/{action}/{id}",
               defaults: new { controller = "Error", action ="MudPlant", id=UrlParameter.Optional},
               namespaces: new string[] { "DirtyGirl.Web.Controllers" }
               );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:new[] { "DirtyGirl.Web.Controllers" }
            );
        }
    }
}