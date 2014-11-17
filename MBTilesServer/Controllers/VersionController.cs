using System.Web.Http;

namespace MBTilesServer.Controllers
{
    public class VersionController:ApiController
    {
        public string GetVersion()
        {
            return "0.1";
        }
    }
}
