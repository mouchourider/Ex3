using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ex3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("display", "display/{ip}/{port}/{time}",
            defaults: new { controller = "Default", action = "display", time = UrlParameter.Optional });

            routes.MapRoute("save", "save/{ip}/{port}/{time}/{sessionT}/{file}",
            defaults: new { controller = "Default", action = "save" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
