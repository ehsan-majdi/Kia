using System.Web.Mvc;

namespace KiaGallery.Web.Areas.DailyReportFinancial
{
    public class DailyReportFinancialAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DailyReportFinancial";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                name: "DailyReportFinancial_api",
                url: "api/dailyReportFinancial/{controller}/{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional },
                namespaces: new[] { "KiaGallery.Web.Areas.DailyReportFinancial.Controllers" }
            );
            
        }
    }
}