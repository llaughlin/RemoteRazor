using System;
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
            var remoteAddress = Request.GetRemoteAddress();
            var localView = View(model);
            if (remoteAddress.IsNullOrWhiteSpace()) return View(model);

            return Json(new {ViewData, localView, RouteData}, JsonRequestBehavior.AllowGet);

            var remotelyRenderedView = VirtualRenderer.RenderView(this, model, remoteAddress);
            return remotelyRenderedView;
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
        public static ActionResult RenderView(Controller controller, RenderingModel model, string address)
        {
            throw new NotImplementedException();
        }
    }
}
    public static class IpControllerExtensions
    {
        public static string GetRemoteAddress(this HttpRequestBase request)
        {
            return request.Headers.Get("RenderingAddress");
        }
    }