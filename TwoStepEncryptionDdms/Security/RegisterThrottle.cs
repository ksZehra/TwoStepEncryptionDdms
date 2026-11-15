using System;
using System.Web;
using System.Web.Caching;

namespace TwoStepEncryptionDdms.Security
{
    public static class RegisterThrottle
    {
        private const int MaxAttempts = 10;
        private static readonly TimeSpan Window = TimeSpan.FromHours(1);

        public static bool IsBlocked(HttpContext context)
        {
            object count = context.Cache[Key(context)];
            return count != null && (int)count >= MaxAttempts;
        }

        public static void RegisterAttempt(HttpContext context)
        {
            string key = Key(context);
            object current = context.Cache[key];
            int count = current == null ? 1 : (int)current + 1;

            context.Cache.Insert(key, count, null, DateTime.UtcNow.Add(Window), Cache.NoSlidingExpiration);
        }

        private static string Key(HttpContext context)
        {
            return "register-throttle:" + context.Request.UserHostAddress;
        }
    }
}
