using System.Web.Http;

namespace MBTilesServer.Controllers
{
    public class VersionController:ApiController
    {
        public string GetVersion()
        {
            return "version 0.1";
        }
    }
}
