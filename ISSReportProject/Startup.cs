using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using ISSReportProject.App_Start;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Unity.WebApi;

[assembly: OwinStartup(typeof(ISSReportProject.Startup))]

namespace ISSReportProject
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            WebApiConfig.Register(config);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            app.UseWebApi(config);
            app.UseCors(CorsOptions.AllowAll);
        }
    }
}
