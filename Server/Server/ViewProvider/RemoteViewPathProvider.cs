using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace Server.ViewProvider
{
    public class RemoteViewPathProvider : VirtualPathProvider
    {

        // virtualPath example >>  "~/Views/View/21EC2020-3AEA-1069-A2DD-08002B30309D.cshtml"
        private static bool IsPathVirtual(string virtualPath)
        {
            var path = VirtualPathUtility.ToAppRelative(virtualPath);

            // returns true if the path is requested by the "ViewController" controller
            return path.StartsWith("~/Views/VIRTUAL/", StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool FileExists(string virtualPath)
        {
            if (!IsPathVirtual(virtualPath)) return Previous.FileExists(virtualPath);

            var file = (SimpleVirtualFile) GetFile(virtualPath);
            return file.Exists;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            return IsPathVirtual(virtualPath) ? new SimpleVirtualFile(virtualPath) : Previous.GetFile(virtualPath);
        }

        // Simply return the virtualPath on every request.
        public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
        {
            if (IsPathVirtual(virtualPath))
            {
                // Returns the virtual path value which is made up of the views GUID value that only
                // changes once the view has been updated - essentially working like an updated FileHash
                // but also the ID of the requested view
                return virtualPath;
            }
            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        private class SimpleVirtualFile : VirtualFile
        {
            private readonly MyDataRepository _dataRepository;
            private string _content;

            public bool Exists => (_content != null);

            public SimpleVirtualFile(string virtualPath)
                : base(virtualPath)
            {
                _dataRepository = new MyDataRepository();
                GetContent();
            }

            public override Stream Open()
            {
                var encoding = new ASCIIEncoding();
                return new MemoryStream(encoding.GetBytes(this._content), false);
            }

            private void GetContent()
            {
                if (IsPathVirtual(VirtualPath) && !VirtualPath.Contains("_ViewStart"))
                {
                    // your GetTemplateByPath method would need to split the virtualPath string to retrieve
                    // proper file
                   this._content = _dataRepository?.GetTemplateByPath(VirtualPath);
                }
            }
        }

        // just implement the default override and return null if the path is a Virtual Path
        public override CacheDependency GetCacheDependency(string virtualPath,
            System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (!IsPathVirtual(virtualPath))
                return Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            return null;
        }
    }


    internal class MyDataRepository
    {
        public string GetTemplateByPath(string virtualPath)
        {
            return 
                "@model Server.Controllers.RenderingModel \r\n"
                   + "< !DOCTYPE html >"
                   + ""
                   + "< html >"
                   + "< head >"
                   + "< title > Client's Index.html</title>"
                   + "</ head >"
                   + "< body >"
                   + "< div >\r\n"
                   + "< h1 > Time: @Model.Time.ToLocalTime().ToLongTimeString() </ h1 > \r\n"
                   + "< div > Data: @Model.Data </ div >\r\n"
                   + "< div > This content was rendered on the server - using CLIENT file</ div >\r\n"
                   + "</ div >\r\n"
                   + "</ body >"
                   + "</ html >";
        }
    }
}