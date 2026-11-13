using System;
using TwoStepEncryptionDdms.Logging;
using System.Data;
using System.Web;
using TwoStepEncryptionDdms.Common;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class Personel : TwoStepEncryptionDdms.Security.BasePage
    {
        readonly AccountService accounts = new AccountService();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Hesap bilgileri yalnızca login olmuş kullanıcıya aittir; user id client'tan (URL) alınmaz (IDOR).
                DataTable dtPersonel = accounts.GetAccountInfo(CurrentUser.Id);

                foreach (DataRow item in dtPersonel.Rows)
                {
                    lbAd.Text = HttpUtility.HtmlEncode(Convert.ToString(item["KAD"]));
                    lbSoyad.Text = HttpUtility.HtmlEncode(Convert.ToString(item["KSOYAD"]));
                    lbEposta.Text = HttpUtility.HtmlEncode(Convert.ToString(item["KEMAIL"]));
                    lbUserName.Text = HttpUtility.HtmlEncode(Convert.ToString(item["KULLANICIADI"]));
                    lbDurum.Text = Convert.ToString(item["KDURUM"]) == "1" ? "Aktif" : "Pasif";
                    lbProfil.Text = HttpUtility.HtmlEncode(Convert.ToString(item["PROFILADI"]));
                    lbYetki.Text = Convert.ToString(item["LISTELEME"]) + Convert.ToString(item["GUNCELLEME"]) + Convert.ToString(item["YENIEKLEME"]) + Convert.ToString(item["GORUNTULEME"]) + Convert.ToString(item["SILME"]).TrimEnd().TrimEnd(',');
                }
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Hesap bilgileri listelenirken hata oluştu!", ex, this.Form);
            }
        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            const string showDialogScript = "$('#sifreModal').modal('show');";
            try
            {
                accounts.ChangePassword(CurrentUser.Id, txtMevcutSifre.Text, txtYeniSifre.Text);
                AppLog.Audit("PasswordChanged");
                ClsMessage.ShowSuccess("Şifre başarıyla değiştirildi", this.Form, showDialogScript);
            }
            catch (UserException ux)
            {
                ClsMessage.ShowError(ux.Message, this.Form, showDialogScript);
            }
            catch (Exception ex)
            {
                ClsMessage.ShowError("Şifre değiştirilirken hata oluştu!", ex, this.Form, "");
            }
        }
    }
}
