using System.Web.Mvc;

namespace KiaGallery.Web.Areas.Bot
{
    public class BotAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Bot";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Bot_default",
                "Bot/{controller}/{action}/{id}",
                defaults: new { controller = "home", action = "index", id = UrlParameter.Optional},
                namespaces: new string[] { "KiaGallery.Web.Areas.Bot.Controllers" }
            );
        }
    }
}