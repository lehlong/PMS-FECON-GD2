using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace SMO
{
    public class JsonNetResult : JsonResult
    {
#pragma warning disable CS0108 // 'JsonNetResult.JsonRequestBehavior' hides inherited member 'JsonResult.JsonRequestBehavior'. Use the new keyword if hiding was intended.
        public JsonRequestBehavior JsonRequestBehavior { get; set; }
#pragma warning restore CS0108 // 'JsonNetResult.JsonRequestBehavior' hides inherited member 'JsonResult.JsonRequestBehavior'. Use the new keyword if hiding was intended.
#pragma warning disable CS0108 // 'JsonNetResult.ContentEncoding' hides inherited member 'JsonResult.ContentEncoding'. Use the new keyword if hiding was intended.
        public Encoding ContentEncoding { get; set; }
#pragma warning restore CS0108 // 'JsonNetResult.ContentEncoding' hides inherited member 'JsonResult.ContentEncoding'. Use the new keyword if hiding was intended.
#pragma warning disable CS0108 // 'JsonNetResult.ContentType' hides inherited member 'JsonResult.ContentType'. Use the new keyword if hiding was intended.
        public string ContentType { get; set; }
#pragma warning restore CS0108 // 'JsonNetResult.ContentType' hides inherited member 'JsonResult.ContentType'. Use the new keyword if hiding was intended.
#pragma warning disable CS0108 // 'JsonNetResult.Data' hides inherited member 'JsonResult.Data'. Use the new keyword if hiding was intended.
        public object Data { get; set; }
#pragma warning restore CS0108 // 'JsonNetResult.Data' hides inherited member 'JsonResult.Data'. Use the new keyword if hiding was intended.
        public JsonSerializerSettings SerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public JsonNetResult()
        {
            Formatting = Formatting.None;
            SerializerSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet
                && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
                                        ? ContentType
                                        : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                var writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }
}