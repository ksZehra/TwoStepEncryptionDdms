using System;
using System.Web;
using System.Web.Caching;

namespace TwoStepEncryptionDdms.Security
{
    public static class LoginThrottle
    {
        private const int MaxAttempts = 5;
        private static readonly TimeSpan Window = TimeSpan.FromMinutes(10);

        private static string Key(string userName, string ip)
        {
            return "login-throttle:" + (userName ?? string.Empty).Trim().ToLowerInvariant() + "|" + ip;
        }

        public static bool IsBlocked(HttpContext context, string userName)
        {
            object count = context.Cache[Key(userName, context.Request.UserHostAddress)];
            return count != null && (int)count >= MaxAttempts;
        }

        public static void RegisterFailure(HttpContext context, string userName)
        {
            string key = Key(userName, context.Request.UserHostAddress);
            object current = context.Cache[key];
            int count = current == null ? 1 : (int)current + 1;

            // Her başarısız denemede süre yenilenir; saldırgan denemeye devam ettikçe block sürer.
            context.Cache.Insert(key, count, null, DateTime.UtcNow.Add(Window), Cache.NoSlidingExpiration);
        }

        public static void Reset(HttpContext context, string userName)
        {
            context.Cache.Remove(Key(userName, context.Request.UserHostAddress));
        }
    }
}
