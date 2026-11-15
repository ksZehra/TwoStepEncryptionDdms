using System;
using TwoStepEncryptionDdms.Logging;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class Register : System.Web.UI.Page
    {
        readonly AccountService accounts = new AccountService();

        protected void Page_Init(object sender, EventArgs e)
        {
            ViewStateUserKey = AuthService.GetAnonymousKey(Context);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnKayitOl_Click(object sender, EventArgs e)
        {
            if (RegisterThrottle.IsBlocked(Context))
            {
                AppLog.AuditWarn("RegisterBlocked");
                lblMessage.Text = "Çok fazla kayıt denemesi yapıldı. Lütfen daha sonra tekrar deneyiniz.";
                return;
            }
            RegisterThrottle.RegisterAttempt(Context);

            string errorMessage = "";
            try
            {
                if (txtSifre.Text != txtTekrarSifre.Text)
                {
                    throw new UserException("Şifre ve şifre tekrarı aynı olmalıdır!");
                }

                accounts.Register(txtAd.Text, txtSoyad.Text, txtEmail.Text, txtKullaniciAdi.Text, txtSifre.Text);
                AppLog.Audit("Register", null, txtKullaniciAdi.Text.Trim());
            }
            catch (UserException ux)
            {
                errorMessage = ux.Message;
            }
            catch (Exception ex)
            {
                errorMessage = "Hesap oluşturulurken hata oluştu! (Kod: " + AppLog.Error(ex, "Kayıt") + ")";
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                Response.Redirect("Login.aspx");
            }

            lblMessage.Text = errorMessage;
        }

        protected void btnGiris_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}
