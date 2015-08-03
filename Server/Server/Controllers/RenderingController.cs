using System;
using System.EnterpriseServices;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using Microsoft.Web.Infrastructure;
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
            var viewPath = this.GetActionViewPath();
            var localView = View(model);
//            if (remoteAddress.IsNullOrWhiteSpace()) return localView;
            return Json(new {localView, viewPath}, JsonRequestBehavior.AllowGet);
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

public static class ControllerExtensions
{
    public static string GetActionViewPath(this Controller controller)
    {
        var actionName = controller.RouteData.GetRequiredString("action");
        var foundView = controller.ViewEngineCollection.FindView(controller.ControllerContext, actionName, null);
        var razorView = (RazorView) foundView.View;
        var viewPath = razorView.ViewPath;

        return viewPath;
    }
}
