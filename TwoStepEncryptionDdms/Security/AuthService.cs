using System;
using System.Data;
using System.Web;
using System.Web.Security;
using TwoStepEncryptionDdms.Data;

namespace TwoStepEncryptionDdms.Security
{
    public static class AuthService
    {
        private const int IdleTimeoutMinutes = 60;
        private const int MaxSessionHours = 8;
        private const string ItemsKey = "__AppUser";
        private const string RememberedUserCookie = "RememberedUser";
        private const string AnonymousKeyCookie = "AnonymousKey";
        private const int RememberedUserDays = 7;
        private const int MaxUserNameLength = 100;

        // Ticket bir session cookie'dir (browser kapanınca sona erer). Idle timeout, Web.config'teki forms timeout ile aynı olmalıdır.
        public static void SignIn(HttpContext context, int userId, string userName, string stamp)
        {
            IssueTicket(context, userId, userName, stamp);
            context.Items.Remove(ItemsKey);
        }

        public static void RenewTicket(HttpContext context, int userId, string userName, string stamp)
        {
            IssueTicket(context, userId, userName, stamp);
        }

        private static void IssueTicket(HttpContext context, int userId, string userName, string stamp)
        {
            DateTime now = DateTime.Now;
            var ticket = new FormsAuthenticationTicket(
                1, userName, now, now.AddMinutes(IdleTimeoutMinutes), false,
                userId + "|" + DateTime.UtcNow.Ticks + "|" + stamp, FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                SameSite = SameSiteMode.Lax
            };

            context.Response.Cookies.Set(cookie);
        }

        public static string GetAnonymousKey(HttpContext context)
        {
            HttpCookie cookie = context.Request.Cookies[AnonymousKeyCookie];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                return cookie.Value;
            }

            string key = Guid.NewGuid().ToString("N");
            context.Response.Cookies.Set(new HttpCookie(AnonymousKeyCookie, key)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                SameSite = SameSiteMode.Lax
            });
            return key;
        }

        public static string GetRememberedUserName(HttpContext context)
        {
            HttpCookie cookie = context.Request.Cookies[RememberedUserCookie];
            if (cookie == null || string.IsNullOrWhiteSpace(cookie.Value))
            {
                return null;
            }

            string userName = HttpUtility.UrlDecode(cookie.Value);
            return string.IsNullOrWhiteSpace(userName) || userName.Length > MaxUserNameLength ? null : userName;
        }

        public static void RememberUserName(HttpContext context, string userName)
        {
            context.Response.Cookies.Set(CreateUserNameCookie(HttpUtility.UrlEncode(userName), DateTime.Now.AddDays(RememberedUserDays)));
        }

        public static void ForgetUserName(HttpContext context)
        {
            context.Response.Cookies.Set(CreateUserNameCookie(string.Empty, DateTime.Now.AddYears(-1)));
        }

        private static HttpCookie CreateUserNameCookie(string value, DateTime expires)
        {
            return new HttpCookie(RememberedUserCookie, value)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                SameSite = SameSiteMode.Lax,
                Expires = expires
            };
        }

        public static void SignOut(HttpContext context)
        {
            FormsAuthentication.SignOut();
            if (context.Session != null)
            {
                context.Session.Clear();
                context.Session.Abandon();
            }

            var expired = new HttpCookie(FormsAuthentication.FormsCookieName, string.Empty)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Path = FormsAuthentication.FormsCookiePath,
                Expires = DateTime.Now.AddYears(-1)
            };
            context.Response.Cookies.Set(expired);
            context.Items.Remove(ItemsKey);
        }

        public static string GetCsrfToken(HttpContext context)
        {
            string token = context.Session["__csrf"] as string;
            if (string.IsNullOrEmpty(token))
            {
                token = Guid.NewGuid().ToString("N");
                context.Session["__csrf"] = token;
            }
            return token;
        }

        public static bool ValidateCsrfToken(HttpContext context, string token)
        {
            string expected = context.Session["__csrf"] as string;
            return !string.IsNullOrEmpty(expected) && !string.IsNullOrEmpty(token)
                   && string.Equals(expected, token, StringComparison.Ordinal);
        }

        public static AppUser GetCurrentUser(HttpContext context)
        {
            if (context.Items.Contains(ItemsKey))
            {
                return context.Items[ItemsKey] as AppUser;
            }

            AppUser user = LoadUser(context);
            context.Items[ItemsKey] = user;
            return user;
        }

        private static AppUser LoadUser(HttpContext context)
        {
            var identity = context.User == null ? null : context.User.Identity as FormsIdentity;
            if (identity == null || !identity.IsAuthenticated || identity.Ticket.Expired)
            {
                return null;
            }

            string userData = identity.Ticket.UserData ?? string.Empty;
            string[] parts = userData.Split('|');
            int userId;
            long signedInTicks;
            if (parts.Length != 3
                || !int.TryParse(parts[0], out userId)
                || !long.TryParse(parts[1], out signedInTicks))
            {
                return null;
            }
            string ticketStamp = parts[2];

            // Idle timeout ticket'ı yeniler; bu kontrol session'ın toplam süresini (absolute timeout) sınırlar.
            if (DateTime.UtcNow - new DateTime(signedInTicks, DateTimeKind.Utc) > TimeSpan.FromHours(MaxSessionHours))
            {
                return null;
            }

            var data = new DataIslemleri();
            try
            {
                data.Open();
                data.AddParameter("@KID", userId);
                DataTable dt = data.QueryTable(
                    @"SELECT U.KULLANICIADI, U.KDURUM, U.KSIFRE, ISNULL(U.KAD,'')+' '+ISNULL(U.KSOYAD,'') ADSOYAD,
                             PS.LIST, PS.NEW, PS.EDIT, PS.[VIEW], PS.[DELETE]
                      FROM TBL_USERS U
                      LEFT JOIN TBL_PROFILSEC PS ON PS.PROFILKODU = U.PROFILKODU
                      WHERE U.KID = @KID");

                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                DataRow first = dt.Rows[0];
                bool active = Convert.ToString(first["KDURUM"]) == "1";
                bool nameMatches = string.Equals(Convert.ToString(first["KULLANICIADI"]), identity.Name, StringComparison.Ordinal);
                bool stampMatches = string.Equals(PasswordHasher.Stamp(Convert.ToString(first["KSIFRE"])), ticketStamp, StringComparison.Ordinal);
                if (!active || !nameMatches || !stampMatches)
                {
                    return null;
                }

                Permission permissions = Permission.None;
                foreach (DataRow row in dt.Rows)
                {
                    if (IsTrue(row["LIST"])) permissions |= Permission.List;
                    if (IsTrue(row["NEW"])) permissions |= Permission.New;
                    if (IsTrue(row["EDIT"])) permissions |= Permission.Edit;
                    if (IsTrue(row["VIEW"])) permissions |= Permission.View;
                    if (IsTrue(row["DELETE"])) permissions |= Permission.Delete;
                }

                return new AppUser(userId, identity.Name, Convert.ToString(first["ADSOYAD"]).Trim(), permissions);
            }
            finally
            {
                data.Dispose();
            }
        }

        private static bool IsTrue(object value)
        {
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }
    }
}
