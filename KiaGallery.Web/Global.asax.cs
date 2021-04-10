using KiaGallery.Common;
using KiaGallery.Model.Context;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace KiaGallery.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (Request.Headers["Authorization"] != null)
            {
                var token = Request.Headers["Authorization"];
                var userId = Auth.CheckToken(token);
                if (userId != null)
                {
                    using (var db = new KiaGalleryContext())
                    {

                        var user = db.UserToken.Where(x => x.AuthoritarianToken == token && x.ExpiredDateTime >= DateTime.Now).Select(x => x.User).Include(x => x.RoleList).SingleOrDefault();
                        if (user != null)
                            HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("KiaGallery", "Forms"), user.RoleList.Select(x => x.Title).ToArray());
                    }
                }
            }
            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                     
                        //let us take out the username now                
                        HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                        cookie.Expires = DateTime.Now.AddMinutes(90);
                        HttpContext.Current.Request.Cookies.Set(cookie);
                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                        var user = JsonConvert.DeserializeObject<User>(ticket.Name);
                        //let us extract the roles from our own custom cookie
                        using (var db = new KiaGalleryContext())
                        {
                            var userEntity = db.User.Where(x => x.Id == user.Id).SingleOrDefault();
                            if (userEntity != null)
                                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity("KiaGallery", "Forms"), userEntity.RoleList.Select(x => x.Title).ToArray());
                        }
                    }
                    catch (Exception ex)
                    {
                        //somehting went wrong
                    }
                }
            }
        }
    }
}
