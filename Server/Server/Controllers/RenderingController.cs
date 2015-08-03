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
            var model = new RenderingModel(JsonConvert.SerializeObject(new { Server, ModelState }, Formatting.Indented));

            //Comented out to always try new Virtual code
            //var remoteAddress = Request.GetRemoteAddress();
            //if (remoteAddress.IsNullOrWhiteSpace()) return View(model);

            var actionName = RouteData.GetRequiredString("action");
            var foundView = ViewEngineCollection.FindView(ControllerContext, actionName, null);
            var razorView = (RazorView)foundView.View;
            var viewPath = razorView.ViewPath;
            return View("~/Views/VIRTUAL/" + viewPath.Substring(1), model);

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

}
    public static class IpControllerExtensions
    {
        public static string GetRemoteAddress(this HttpRequestBase request)
        {
            return request.Headers.Get("RenderingAddress");
        }
    }