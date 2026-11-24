using System;
using TwoStepEncryptionDdms.Logging;
using System.Web;
using System.Web.UI;

namespace TwoStepEncryptionDdms.Security
{
    public abstract class BasePage : Page
    {
        protected virtual Permission RequiredPermission
        {
            get { return Permission.None; }
        }

        public AppUser CurrentUser
        {
            get { return AuthService.GetCurrentUser(Context); }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (CurrentUser == null)
            {
                AuthService.SignOut(Context);
                Response.Redirect("~/Login.aspx", true);
                return;
            }

            EnsurePermission(RequiredPermission);

            // Anahtar yalnızca DataView kullanılırken session'da kalır.
            if (!(this is DataView))
            {
                TableKeyService.ForgetAll(Context);
            }

            // CSRF: ViewState/event validation kullanıcıya bağlanır; başka bir site bu form'u submit edemez.
            ViewStateUserKey = CurrentUser.Id.ToString();
        }

        protected void EnsurePermission(Permission permission)
        {
            if (permission == Permission.None || CurrentUser.Has(permission))
            {
                return;
            }

            AppLog.AuditWarn("AccessDenied", permission.ToString());

            Response.Clear();
            Response.StatusCode = 403;
            Response.TrySkipIisCustomErrors = true;
            Response.ContentType = "text/plain; charset=utf-8";
            Response.Write("Bu işlem için yetkiniz bulunmamaktadır!");
            Response.End();
        }
    }
}
