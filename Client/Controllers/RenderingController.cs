using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features;
using Microsoft.Net.Http.Client;

namespace Client.Controllers
{
    public class RenderingController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var localIp = this.GetLocalIpAddress();

            var thorRequest = GetRequestFromContext(Context);
            thorRequest.AttachIpAddress(localIp);

            SetRequestHost(thorRequest, Context, GetRemoteUrl());
            //            Logger.LogInformation($"Proxy request={externRequest.ToJson()}");
            var response = await new HttpClient().SendAsync(thorRequest);
            await SetResponse(Context.Response, response);

            return View();
        }
        public async Task SetResponse(HttpResponse response, HttpResponseMessage responseMessage)
        {
            if (ResponseHasBody(response))
            {
                throw new Exception("This only works if 404's bodies are empty.");
            }

            response.Headers.Clear();
            // borrowed from https://github.com/aspnet/Mvc/blob/bd03142daba3854ac976906588bcaa1dc98accd0/src/Microsoft.AspNet.Mvc.WebApiCompatShim/Formatters/HttpResponseMessageOutputFormatter.cs
            // Ignore the Transfer-Encoding header if it is just "chunked".
            // We let the host decide about whether the response should be chunked or not.
            if (responseMessage.Headers.TransferEncodingChunked == true &&
                responseMessage.Headers.TransferEncoding.Count == 1)
            {
                responseMessage.Headers.TransferEncoding.Clear();
            }
            // Copy the response content headers only after ensuring they are complete.
            // We ask for Content-Length first because HttpContent lazily computes this
            // and only afterwards writes the value into the content headers.
            var unused = responseMessage.Content.Headers.ContentLength;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers.Add(header.Key, header.Value.ToArray());
            }
            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers.Add(header.Key, header.Value.ToArray());
            }

            response.StatusCode = (int)responseMessage.StatusCode;
            await responseMessage.Content.CopyToAsync(response.Body);
        }
        private static void SetRequestHost(HttpRequestMessage request, HttpContext context, string host)
        {
            var httpRequest = context.Request;

            request.RequestUri = new UriBuilder(UriHelper.Encode(
                httpRequest.Scheme,
                new HostString(host),
                httpRequest.PathBase,
                httpRequest.Path,
                httpRequest.QueryString)
                ).Uri;
        }
        private bool ResponseHasBody(HttpResponse response)
        {
            if (response.ContentLength.GetValueOrDefault() == 0)
            {
                return false;
            }

            if (response.Body.CanSeek)
            {
                return response.Body.Position != 0;
            }

            if (response.Body.CanRead)
            {
                return new StreamReader(response.Body).ReadLine().Any();
            }

            //            Logger<>.LogWarning($"Could not determine content of HttpResponse body. Response={response.ToJson()}");
            return false;
        }
        public static HttpRequestMessage GetRequestFromContext(HttpContext httpContext)
        {
            var contextRequest = httpContext.Request;

            // TODO Content/Body of the message
            var request = new HttpRequestMessage
            {
                // TODO Should make this more robust. For instance, an HTTP method that isn't in the enum will break this.
                Method = new HttpMethod(contextRequest.Method),
            };

            foreach (var header in contextRequest.Headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }
        public static string GetRemoteUrl()
        {
            return "http://localhost:12977";
        }
    }
    public class RenderingModel
    {
        public RenderingModel(string origin)
        {
            Origin = origin;
            Time = DateTime.Now;
        }

        public DateTime Time { get; set; }

        public string Origin { get; set; }
    }

    public static class Extensions
    {
        public static void AttachIpAddress(this HttpRequestMessage request, string ipAddress)
        {
            var query = request.GetPathAndQueryProperty();
        }

        public static string GetLocalIpAddress(this Controller controller)
        {
            return controller.Context.GetFeature<IHttpConnectionFeature>()?.LocalIpAddress.ToString();
        }


    }
}
