using System.Web.Mvc;
using System.Web.Routing;

namespace IzdavanjeFaktura
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{iditem}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, iditem = UrlParameter.Optional }
            );
        }
    }
}
