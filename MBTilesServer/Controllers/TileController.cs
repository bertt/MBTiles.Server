using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MBTiles.Provider;
using Newtonsoft.Json;

namespace MBTilesServer.Controllers
{
    public class TileController:ApiController
    {
        [Route("{id}/{level:int:min(0)}/{col:int:min(0)}/{row:int:min(0)}.{ext}")]
        public HttpResponseMessage GetTile(string id,string level, int col, int row,string ext)
        {
            var app = HttpContext.Current.Request.ApplicationPath;
            var physicalPath = HttpContext.Current.Request.MapPath(app);
             if (!physicalPath.EndsWith(@"\")) physicalPath+= @"\";

            var mbtileExtension = ConfigurationManager.AppSettings["mbtileExtension"];
            // first read the config file
            var mbtileconfig = ConfigurationManager.AppSettings["MBTileConfig"];
            var conf = LoadJson(physicalPath + mbtileconfig);
            var tileset = GetTileSetFromConfig(conf,id);
            if (tileset != null)
            {
                var mbtilefile = GetMbTileFile(tileset,level,mbtileExtension);

                if (mbtilefile != null)
                {
                    var image = GetTileImage(mbtilefile, level, col, row);

                    if (image != null)
                    {
                        var memoryStream = new MemoryStream();
                        var imageFormat = ImageFormat.Png;

                        image.Save(memoryStream, imageFormat);
                        const string contentType = "image/png";
                        return GetHttpResponseMessage(memoryStream.ToArray(), contentType, HttpStatusCode.OK);
                    }
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "image not found in mbtile" + mbtilefile };
                }
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound, ReasonPhrase = "tileset not found in config: " + id};
        }


        private string GetMbTileFile(dynamic tileset,string level, string mbtileExtension)
        {
            var mbtilefile = (string)tileset.source;
            if (mbtilefile.Contains("app_data"))
            {
                mbtilefile = mbtilefile.Replace("app_data", AppDomain.CurrentDomain.GetData("DataDirectory").ToString());
            }

            if (tileset.type == "dir")
            {
                mbtilefile += level + "." + mbtileExtension;
            }

            return mbtilefile;
        }


        internal dynamic GetTileSetFromConfig(dynamic conf, string id)
        {
            var p1 = conf.tilesets;
            foreach (var t in p1)
            {
                if (t.id == id)
                {
                    return t;
                }
            }
            return null;
        }


        public dynamic LoadJson(string file)
        {
            using (var r = new StreamReader(file))
            {
                var json = r.ReadToEnd();
                var res = JsonConvert.DeserializeObject(json);
                return res;
            }

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