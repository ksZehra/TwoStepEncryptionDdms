using System;
using TwoStepEncryptionDdms.Logging;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class Login : System.Web.UI.Page
    {
        readonly AccountService accounts = new AccountService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && User.Identity.IsAuthenticated && AuthService.GetCurrentUser(Context) != null)
            {
                Response.Redirect("Anasayfa.aspx");
            }

            if (!IsPostBack)
            {
                string rememberedUser = AuthService.GetRememberedUserName(Context);
                if (rememberedUser != null)
                {
                    txtKAdi.Text = rememberedUser;
                    chboxBeniHatirla.Checked = true;
                }
            }
        }

        protected void btnGirisYap_Click(object sender, EventArgs e)
        {
            bool loginAccess = false;

            try
            {
                string userName = txtKAdi.Text.Trim();
                string pass = txtPassword.Text.TrimEnd();

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(pass))
                {
                    lblMesaj.Text = "Kullanıcı adı veya şifre doğru değil. Lütfen tekrar deneyiniz!!!";
                    return;
                }

                if (LoginThrottle.IsBlocked(Context, userName))
                {
                    AppLog.AuditWarn("LoginBlocked", null, userName);
                    lblMesaj.Text = "Çok fazla başarısız deneme yapıldı. Lütfen birkaç dakika sonra tekrar deneyiniz.";
                    return;
                }

                AuthenticatedUser user = accounts.Authenticate(userName, pass);
                if (user != null)
                {
                    LoginThrottle.Reset(Context, userName);
                    AppLog.Audit("LoginSuccess", null, user.UserName);
                    AuthService.SignIn(Context, user.Id, user.UserName);

                    if (chboxBeniHatirla.Checked)
                    {
                        AuthService.RememberUserName(Context, user.UserName);
                    }
                    else
                    {
                        AuthService.ForgetUserName(Context);
                    }

                    loginAccess = true;
                }
                else
                {
                    LoginThrottle.RegisterFailure(Context, userName);
                    AppLog.AuditWarn("LoginFailed", null, userName);
                    lblMesaj.Text = "Kullanıcı adı veya şifre doğru değil. Lütfen tekrar deneyiniz!!!";
                }
            }
            catch (Exception ex)
            {
                lblMesaj.Text = "Giriş yaparken hata oluştu! (Kod: " + AppLog.Error(ex, "Giriş") + ")";
            }

            if (loginAccess)
            {
                Response.Redirect("Anasayfa.aspx");
            }
        }

        protected void btnKayitOl_Click(object sender, EventArgs e)
        {
            Response.Redirect("Register.aspx");
        }

        protected void btnEmailAktif_Click(object sender, EventArgs e)
        {
            Response.Redirect("HesabiAktifEt.aspx");
        }
    }
}
