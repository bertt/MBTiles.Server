using System.Web.Http;
using MBTilesServer.Filters;

namespace MBTilesServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional }
            );
            config.Filters.Add(new ElmahErrorAttribute());

        }
    }
}
