using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Api.Models
{
    public class JsonResult : ActionResult {

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        public object Data { get; set; }

        //public JsonSerializerSettings Settings { get; private set; }

        public bool ReadableFormat { get; set; }

        public JsonResult(object data) {
            //Settings = new JsonSerializerSettings();
            //Settings.NullValueHandling = NullValueHandling.Include;

            //var dateTimeConverter = new IsoDateTimeConverter();
            ////这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式
            //dateTimeConverter.DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
            //Settings.Converters.Add(dateTimeConverter);
            Data = data;
        }

        public override void ExecuteResult(ControllerContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType)) 
                response.ContentType = ContentType;
            else 
                response.ContentType = "application/json";
            
            if (ContentEncoding != null) 
                response.ContentEncoding = ContentEncoding;

            var json = SerializeObject(Data);

            response.Write(json);
        }

        private string SerializeObject(object o) {
            return JsonConvert.SerializeObject(o);
        }

    }
}