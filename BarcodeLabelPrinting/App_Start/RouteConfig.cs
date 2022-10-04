using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BarcodeLabelPrinting
{
	public class RouteConfig
	{
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "GetAllPoints",
                url: "handlers/GetAllPoints.ashx",
                defaults: new { controller = "Maps", action = "GetAllPoints", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "GetPoint",
                url: "handlers/GetPoint.ashx",
                defaults: new { controller = "Maps", action = "GetPoint", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Handlers",
                url: "handlers/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
