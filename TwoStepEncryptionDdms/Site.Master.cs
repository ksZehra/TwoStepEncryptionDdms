using System;
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
            liTabloOlustur.Visible = user.Has(Permission.New);
            liKayitlar.Visible = user.Has(Permission.List);

            linkHesap.HRef = "Personel.aspx";

            SetActiveNavigation();
        }

        // Menüde bulunulan page'i işaretler; permission görünürlüğü yukarıdaki kurallarla aynen korunur.
        private void SetActiveNavigation()
        {
            string path = (Request.AppRelativeCurrentExecutionFilePath ?? string.Empty).ToLowerInvariant();

            liAnasayfa.Attributes["class"] = IsPage(path, "anasayfa") ? "active" : "";
            liTablolar.Attributes["class"] = IsPage(path, "tabloschemalist") ? "active" : "";
            liTabloOlustur.Attributes["class"] = IsPage(path, "tablonew") ? "active" : "";
            liKayitlar.Attributes["class"] =
                IsPage(path, "tablodatalist") ||
                IsPage(path, "dataview") ||
                IsPage(path, "dataeditlist") ||
                IsPage(path, "dataeditnew")
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
            AuthService.SignOut(Context);
            Response.Redirect("Login.aspx");
        }
    }
}
