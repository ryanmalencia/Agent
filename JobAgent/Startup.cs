using Owin;
using System.Web.Http;

namespace JobAgent
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            HttpConfiguration config = new HttpConfiguration();
            //config.Routes.MapHttpRoute(name: "DefaultApi", routeTemplate: "api/{controller}/{action}/{id}", defaults: new { id = RouteParameter.Optional });
            config.MapHttpAttributeRoutes();
            config.EnableCors();
            appBuilder.UseWebApi(config);
        }
    }
}