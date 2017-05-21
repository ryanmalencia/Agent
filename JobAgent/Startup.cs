using Owin;
using Microsoft.Owin;
using System.Web.Http;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(JobAgent.Startup))]

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
            var Config = new HubConfiguration();
            Config.EnableJSONP = true;
            appBuilder.MapSignalR(Config);
        }
    }
}