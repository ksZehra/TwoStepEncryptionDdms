using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TwoStepEncryptionDdms.Security
{
    public sealed class DatasetLink
    {
        private const string QueryKey = "q";
        private const string Purpose = "DatasetLink.v1";

        public string TblId { get; private set; }
        public string TblAd { get; private set; }
        public string Type { get; private set; }
        public string SatirId { get; private set; }
        public bool RecallKey { get; private set; }

        public static string Build(string page, object tblId, object tblAd, string type = null, string satirId = null, bool recallKey = false)
        {
            string payload = string.Join("\n",
                Convert.ToString(tblId), Convert.ToString(tblAd), type ?? "", satirId ?? "", recallKey ? "1" : "0");

            // Token MachineKey ile encrypt ve sign edilir; purpose'a user id eklendiği için başka kullanıcıda çözülemez.
            byte[] token = MachineKey.Protect(Encoding.UTF8.GetBytes(payload), Purpose, UserPurpose());
            return page + "?" + QueryKey + "=" + HttpServerUtility.UrlTokenEncode(token);
        }

        public static DatasetLink Read(HttpRequest request)
        {
            string raw = request.QueryString[QueryKey];
            if (string.IsNullOrWhiteSpace(raw))
                throw new UserException("Bağlantı geçersiz. Lütfen listeden tekrar seçin.");

            string[] parts;
            try
            {
                byte[] data = MachineKey.Unprotect(HttpServerUtility.UrlTokenDecode(raw), Purpose, UserPurpose());
                parts = Encoding.UTF8.GetString(data).Split('\n');
            }
            catch (Exception)
            {
                parts = null;
            }

            if (parts == null || parts.Length != 5)
                throw new UserException("Bağlantı geçersiz veya süresi dolmuş. Lütfen listeden tekrar seçin.");

            return new DatasetLink
            {
                TblId = parts[0],
                TblAd = parts[1],
                Type = parts[2],
                SatirId = parts[3],
                RecallKey = parts[4] == "1"
            };
        }

        private static string UserPurpose()
        {
            AppUser user = AuthService.GetCurrentUser(HttpContext.Current);
            return "user:" + (user == null ? "0" : user.Id.ToString());
        }
    }
}
