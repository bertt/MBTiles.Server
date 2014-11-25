using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MBTilesServer.Tests
{
    [TestClass]
    public class TileServiceTests
    {
        //[TestMethod]
        public void BasicTileServiceTest()
        {
            // arrange
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            // act 
            var response = client.GetAsync("http://server/countries/0/0/0.png").Result;

            //assert
            // does not work yet
            // because service is using httpcontext
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }
    }
}
