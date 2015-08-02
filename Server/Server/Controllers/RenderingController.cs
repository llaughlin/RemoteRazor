using System;
using System.Data.Entity.Core.Objects;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Server.Controllers
{
    public class RenderingController : Controller
    {
        // GET: Rendering
        public ActionResult Index()
        {
            var model = new RenderingModel(JsonConvert.SerializeObject(new { Server, ModelState}, Formatting.Indented));
            var remoteIp = Request.GetRemoteIp();
            return View(model);
            if (!remoteIp.IsNullOrWhiteSpace())
            {
                var remotelyRenderedView = VirtualRenderer.RenderView(this, model, remoteIp);
            }
            return View(model);
        }
    }

    public class RenderingModel
    {
        public RenderingModel(string data)
        {
            Data = data;
            Time = DateTime.UtcNow;
        }

        public DateTime Time { get; set; }

        public string Data { get; set; }
    }

    public class VirtualRenderer
    {
        public static ActionResult RenderView(Controller controller, RenderingModel address, string ipAddress)
        {
            throw new System.NotImplementedException();
        }
    }
}
    public static class IpControllerExtensions
    {
        public static string GetRemoteIp(this HttpRequestBase request)
        {
            return request.Headers.Get("RenderingAddress");
        }
    }