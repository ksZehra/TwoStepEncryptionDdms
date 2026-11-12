using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using TwoStepEncryptionDdms.Logging;

namespace TwoStepEncryptionDdms
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            AppLog.Initialize();

            try
            {
                AppConfig.Validate();
            }
            catch (Exception ex)
            {
                AppLog.Error(ex, "Yapılandırma doğrulaması");
                throw;
            }

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Exception error = Server.GetLastError();
            if (error is HttpUnhandledException && error.InnerException != null)
            {
                error = error.InnerException;
            }

            AppLog.Error(error, "Yakalanmayan hata");
        }
    }
}
