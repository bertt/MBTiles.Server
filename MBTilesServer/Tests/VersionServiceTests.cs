using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MBTilesServer.Tests
{
    [TestClass]
    public class VersionServiceTests
    {
        [TestMethod]
        public void FirstServiceTest()
        {
            // arrange
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            var server = new HttpServer(config);
            var client = new HttpClient(server);

            // act 
            var response = client.GetAsync("http://server/api/version").Result;

            //assert 
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
            var answer = response.Content.ReadAsStringAsync().Result;
            Assert.IsTrue(answer.Contains("version")); 
        }
    }
}