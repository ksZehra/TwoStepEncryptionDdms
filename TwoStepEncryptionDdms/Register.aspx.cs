using System;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms
{
    public partial class Register : System.Web.UI.Page
    {
        readonly AccountService accounts = new AccountService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnKayitOl_Click(object sender, EventArgs e)
        {
            string errorMessage = "";
            try
            {
                accounts.Register(txtAd.Text, txtSoyad.Text, txtEmail.Text, txtKullaniciAdi.Text, txtSifre.Text);
            }
            catch (UserException ux)
            {
                errorMessage = ux.Message;
            }
            catch (Exception)
            {
                errorMessage = "Hesap oluşturulurken hata oluştu!";
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                Response.Redirect("Login.aspx");
            }

            lblMessage.Text = errorMessage;
        }

        protected void btnGiriş_Click(object sender, EventArgs e)
        {
            Response.Redirect("Login.aspx");
        }
    }
}
