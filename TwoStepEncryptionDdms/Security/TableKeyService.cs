using System;
using TwoStepEncryptionDdms.Logging;
using System.Web;
using TwoStepEncryptionDdms.Data;

namespace TwoStepEncryptionDdms.Security
{
    // Key'in kendisi saklanmaz; TBL_USERAUTHORIZATIONS.KEYVERIFIER'da PBKDF2 verification değeri tutulur.
    // Verify edilen key yalnızca ilgili session'da hatırlanır.
    public static class TableKeyService
    {
        private const string SessionPrefix = "__tblkey:";
        private const string VerifierPrefix = "PBKDF2$";

        public static string CreateVerifier(string key)
        {
            return PasswordHasher.Hash(key);
        }

        public static bool HasKey(DataIslemleri data, string tblAd)
        {
            data.AddParameter("@TBLAD", tblAd);
            string stored = Convert.ToString(data.QueryScalar(
                "SELECT ISNULL(KEYVERIFIER,'') FROM TBL_USERAUTHORIZATIONS WHERE TBLAD=@TBLAD"));
            return !string.IsNullOrWhiteSpace(stored);
        }

        // Hatalı denemeler geçici olarak block edilir; eski plain text key ilk doğru girişte verification değerine taşınır.
        public static bool Verify(HttpContext context, DataIslemleri data, string tblAd, string key)
        {
            string throttleKey = "tablekey:" + tblAd;
            if (LoginThrottle.IsBlocked(context, throttleKey))
            {
                AppLog.AuditWarn("KeyBlocked", tblAd);
                throw new UserException("Çok fazla hatalı anahtar denemesi yapıldı. Lütfen birkaç dakika sonra tekrar deneyiniz.");
            }

            bool ok = false;
            if (!string.IsNullOrEmpty(key))
            {
                data.AddParameter("@TBLAD", tblAd);
                string stored = Convert.ToString(data.QueryScalar(
                    "SELECT ISNULL(KEYVERIFIER,'') FROM TBL_USERAUTHORIZATIONS WHERE TBLAD=@TBLAD"));

                if (stored.StartsWith(VerifierPrefix, StringComparison.Ordinal))
                {
                    PasswordVerifyResult result = PasswordHasher.Verify(key, stored);
                    ok = result != PasswordVerifyResult.Failed;
                    if (result == PasswordVerifyResult.SuccessRehashNeeded)
                    {
                        UpdateVerifier(data, tblAd, key);
                    }
                }
                else if (stored.Length > 0 && PasswordHasher.SecureEquals(stored, key))
                {
                    ok = true;
                    UpdateVerifier(data, tblAd, key);
                }
            }

            if (ok)
            {
                LoginThrottle.Reset(context, throttleKey);
            }
            else
            {
                AppLog.AuditWarn("KeyRejected", tblAd);
                LoginThrottle.RegisterFailure(context, throttleKey);
            }
            return ok;
        }

        private static void UpdateVerifier(DataIslemleri data, string tblAd, string key)
        {
            data.AddParameter("@V", CreateVerifier(key));
            data.AddParameter("@TBLAD", tblAd);
            data.Execute("UPDATE TBL_USERAUTHORIZATIONS SET KEYVERIFIER=@V WHERE TBLAD=@TBLAD");
        }

        public static void Remember(HttpContext context, string tblAd, string key)
        {
            context.Session[SessionPrefix + tblAd] = key;
        }

        public static string Recall(HttpContext context, string tblAd)
        {
            return context.Session[SessionPrefix + tblAd] as string;
        }
    }
}
