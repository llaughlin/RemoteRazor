using System;
using System.Data.Entity.Core.Objects;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace Server.Controllers
{
    public class RenderingController : Controller
    {
        // GET: Rendering
        public ActionResult Index()
        {
            var model = new RenderingModel("on the server");
            var remoteIp = Request.GetRemoteIp();
            if (!remoteIp.IsNullOrWhiteSpace())
            {
                var remotelyRenderedView = VirtualRenderer.RenderView(this, model, remoteIp);
            }
            return View(model);
        }
    }

    public class RenderingModel
    {
        public RenderingModel(string origin)
        {
            Origin = origin;
            Time = DateTime.UtcNow;
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
}
    public static class IpControllerExtensions
    {
        public static string GetRemoteIp(this HttpRequestBase request)
        {
            return request.Params.Get("RemoteRenderIp");
        }
    }