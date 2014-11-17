using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using MBTilesServer.Models;

namespace MBTilesServer.Controllers
{
    public class TileController:ApiController
    {
        [Route("{level:int:min(0)}/{col:int:min(0)}/{row:int:min(0)}.{ext}")]
        public HttpResponseMessage GetTile(string level, int col, int row,string ext)
        {
            var mbtilefile = ConfigurationManager.AppSettings["MBTileFile"];

            if (mbtilefile.Contains("{level}"))
            {
                mbtilefile = mbtilefile.Replace("{level}", level);
            }

            mbtilefile= HostingEnvironment.MapPath("~/App_Data/" + mbtilefile);

            var image = GetTileImage(mbtilefile, level, col, row);

            if (image != null)
            {
                var memoryStream = new MemoryStream();
                var imageFormat = ImageFormat.Png;

                image.Save(memoryStream, imageFormat);
                const string contentType = "image/png";
                return GetHttpResponseMessage(memoryStream.ToArray(), contentType, HttpStatusCode.OK);
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        private Image GetTileImage(string mbtilefile, string level, int col, int row)
        {
            if (File.Exists(mbtilefile))
            {
                var connectionString = string.Format("Data Source={0}", mbtilefile);
                var mbTileProvider = new MBTileProvider(connectionString);
                var image = mbTileProvider.GetTile(level, col, row);
                return image;
            }
            return null;
        }

        public static HttpResponseMessage GetHttpResponseMessage(byte[] content, string contentType, HttpStatusCode code)
        {
            var httpResponseMessage = new HttpResponseMessage { Content = new ByteArrayContent(content) };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            return httpResponseMessage;
        }

    }
}