using System;
using System.Data.Entity.Core.Objects;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace Server.Controllers
{
    public class RenderingController : Controller
    {
        // GET: Rendering
        public ActionResult Index()
        {
            var model = new RenderingModel("on the server", DateTime.Now);
            var ipAddress = this.DetectRequestIp();
            if (!ipAddress.IsNullOrWhiteSpace())
            {
                var remotelyRenderedView = VirtualRenderer.RenderView(this, model, ipAddress);
            }
            return View(model);
        }
    }

    public class RenderingModel
    {
        public RenderingModel(string origin, DateTime now)
        {
            Origin = origin;
            Time = now;
        }

        public DateTime Time { get; set; }

        public string Origin { get; set; }
    }

    public class VirtualRenderer
    {
        public static ActionResult RenderView(Controller controller, RenderingModel address, string ipAddress)
        {
            throw new System.NotImplementedException();
        }
    }
    public static class IpControllerExtensions
    {
        public static string DetectRequestIp(this Controller controller)
        {
            return controller.Request.Params.Get("designerIp");
        }
    }
}