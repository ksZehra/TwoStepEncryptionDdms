using System;
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
                    lblMesaj.Text = "Çok fazla başarısız deneme yapıldı. Lütfen birkaç dakika sonra tekrar deneyiniz.";
                    return;
                }

                AuthenticatedUser user = accounts.Authenticate(userName, pass);
                if (user != null)
                {
                    LoginThrottle.Reset(Context, userName);
                    AuthService.SignIn(Context, user.Id, user.UserName, user.FullName, chboxBeniHatirla.Checked);
                    loginAccess = true;
                }
                else
                {
                    LoginThrottle.RegisterFailure(Context, userName);
                    lblMesaj.Text = "Kullanıcı adı veya şifre doğru değil. Lütfen tekrar deneyiniz!!!";
                }
            }
            catch (Exception)
            {
                lblMesaj.Text = "Giriş yaparken hata oluştu!";
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
