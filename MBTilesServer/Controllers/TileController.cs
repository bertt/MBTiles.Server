using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using MBTiles.Provider;

namespace MBTilesServer.Controllers
{
    public class TileController:ApiController
    {
        [Route("{level:int:min(0)}/{col:int:min(0)}/{row:int:min(0)}.{ext}")]
        [Route("{mbtiles}/{level:int:min(0)}/{col:int:min(0)}/{row:int:min(0)}.{ext}")]
        public HttpResponseMessage GetTile(string level, int col, int row,string ext,string mbtiles=null)
        {
            var mbtileExtension = ConfigurationManager.AppSettings["mbtileExtension"];
            string mbtilefile;

            if (mbtiles == null)
            {
                var mbtiledirectory = ConfigurationManager.AppSettings["mbtiledirectory"];
                mbtilefile = mbtiledirectory + level + "." + mbtileExtension;
            }
            else
            {
                mbtilefile = mbtiles + "." + mbtileExtension;
               // mbtilefile = HostingEnvironment.MapPath("~/App_Data/" + mbtilefile);
                mbtilefile = AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + @"\" + mbtilefile;
            }

            var image = GetTileImage(mbtilefile, level, col, row);

            if (image != null)
            {
                var memoryStream = new MemoryStream();
                var imageFormat = ImageFormat.Png;

                image.Save(memoryStream, imageFormat);
                const string contentType = "image/png";
                return GetHttpResponseMessage(memoryStream.ToArray(), contentType, HttpStatusCode.OK);
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "file not found?" + mbtilefile};
        }

        private Image GetTileImage(string mbtilefile, string level, int col, int row)
        {
            if (File.Exists(mbtilefile))
            {
                var connectionString = string.Format("Data Source={0}", mbtilefile);
                var mbTileProvider = new MbTileProvider(connectionString);
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