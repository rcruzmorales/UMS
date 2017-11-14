using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace UMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Administration",
                url: "Administration/{action}/{id}",
                defaults: new { controller = "Administration", action = "Index", id = UrlParameter.Optional }

            );

            routes.MapRoute(
                name: "Client",
                url: "Client/{action}/{id}",
                defaults: new { controller = "Client", action = "Index", id = UrlParameter.Optional }

            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "User", action = "Login", id = UrlParameter.Optional }

            );

        }
    }
}
