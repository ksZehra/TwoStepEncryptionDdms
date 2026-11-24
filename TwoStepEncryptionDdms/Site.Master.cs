using System;
using TwoStepEncryptionDdms.Logging;
using System.Web;
using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            AppUser user = AuthService.GetCurrentUser(Context);
            if (user == null)
            {
                return;
            }

            lblSession.Text = "Hoşgeldin " + HttpUtility.HtmlEncode(user.FullName);

            if (user.Permissions == Permission.None)
            {
                lblErrorMsg.Text = "Sistemde kayıtlı profiliniz bulunmamaktadır!!!";
            }

            liTablolar.Visible = user.Has(Permission.List);
            liKayitlar.Visible = user.Has(Permission.List);

            linkHesap.HRef = "Account.aspx";

            SetActiveNavigation();
        }

        // Menüde bulunulan page'i işaretler; permission görünürlüğü yukarıdaki kurallarla aynen korunur.
        private void SetActiveNavigation()
        {
            string path = (Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();

            liDashboard.Attributes["class"] = IsPage(path, "dashboard") ? "active" : "";
            liTablolar.Attributes["class"] = IsPage(path, "tableschemalist") || IsPage(path, "tablenew") ? "active" : "";
            liKayitlar.Attributes["class"] =
                IsPage(path, "record") ||
                IsPage(path, "dataview")
                    ? "active"
                    : "";
        }

        private static bool IsPage(string path, string pageName)
        {
            return path.EndsWith("/" + pageName, StringComparison.OrdinalIgnoreCase) ||
                   path.EndsWith("/" + pageName + ".aspx", StringComparison.OrdinalIgnoreCase);
        }

        protected void btnCikisYap_Click(object sender, EventArgs e)
        {
            AppLog.Audit("Logout");
            AuthService.SignOut(Context);
            Response.Redirect("Login.aspx");
        }
    }
}
