using System.IO;
using System.Web;
using System.Web.Hosting;

namespace TwoStepEncryptionDdms.Common
{
    public static class AssetVersion
    {
        // Dosyanın son değişiklik zamanını "?v=" olarak ekler; dosya değişince browser cache'i atlanır.
        public static string Url(string virtualPath)
        {
            string url = VirtualPathUtility.ToAbsolute(virtualPath);

            try
            {
                string physicalPath = HostingEnvironment.MapPath(virtualPath);
                if (physicalPath == null || !File.Exists(physicalPath))
                    return url;

                return url + "?v=" + File.GetLastWriteTimeUtc(physicalPath).Ticks.ToString("x");
            }
            catch
            {
                return url;
            }
        }
    }
}
