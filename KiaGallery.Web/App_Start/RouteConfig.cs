using System.Web.Mvc;
using System.Web.Routing;

namespace KiaGallery.Web
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

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Marquis",
                url: "marquisfile/download/file/{fileId}/{fileName}",
                defaults: new { controller = "marquisFile", action = "downloadFile" },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Image",
                url: "image/{type}/{size}/{fileName}",
                defaults: new { controller = "upload", action = "resizeImage" },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            );

            routes.MapRoute(
                name: "PersonDescTmpl",
                url: "PersonDescTmpl/{action}/{id}",
                defaults: new { controller = "PersonJobDescriptionTemplate", action = "add", id = UrlParameter.Optional },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            );

            routes.MapRoute(
                name: "financialBranchSetting",
                url: "financialBranch/SaveSetting/{value}",
                defaults: new { controller = "financialBranch", action = "saveSetting" },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional },
                namespaces: new string[] { "KiaGallery.Web.Controllers" }
            ); 

        }
    }
}
