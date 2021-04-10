using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KiaGallery.Tracking
{

    /// <summary>
    /// تنظیمات مسیریابی
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// ثبت مسیریابی
        /// </summary>
        /// <param name="routes">شی مربوط به مسیریابی</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute(
               name: "Image",
               url: "image/{type}/{size}/{fileName}",
               defaults: new { controller = "upload", action = "resizeImage" },
               namespaces: new string[] { "KiaGallery.Web.Controllers" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "tracking", action = "trackLogin", id = UrlParameter.Optional },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            );
        }
    }
}
