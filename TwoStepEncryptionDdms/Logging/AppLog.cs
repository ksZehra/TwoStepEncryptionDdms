using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Serilog;
using Serilog.Events;

namespace TwoStepEncryptionDdms.Logging
{
    public static class AppLog
    {
        private const string Template =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level:u3} | {User} | {Ip} | {Page} | {Message:lj}{NewLine}{Exception}";
        private const int RetainedDays = 30;
        private const int MaxTextLength = 500;

        private static ILogger appLogger = Serilog.Core.Logger.None;
        private static ILogger auditLogger = Serilog.Core.Logger.None;

        public static void Initialize()
        {
            try
            {
                string folder = Path.Combine(HttpRuntime.AppDomainAppPath, "App_Data", "logs");
                Directory.CreateDirectory(folder);

                appLogger = Create(Path.Combine(folder, "app-.log"), "app");
                auditLogger = Create(Path.Combine(folder, "audit-.log"), "audit");

                Info("Uygulama başlatıldı");
            }
            catch (Exception ex)
            {
                Trace.TraceError("Loglama başlatılamadı: " + ex.Message);
            }
        }

        // Exception'ı app log'a yazar ve kullanıcıya gösterilecek kısa error code'u döndürür.
        public static string Error(Exception ex, string context = null)
        {
            string code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();
            Write(appLogger, LogEventLevel.Error, null, "[" + code + "] " + Describe(ex, context), ex, null, code);
            return code;
        }

        public static void Warn(string message)
        {
            Write(appLogger, LogEventLevel.Warning, null, Clean(message), null);
        }

        public static void Info(string message)
        {
            Write(appLogger, LogEventLevel.Information, null, Clean(message), null);
        }

        // userName verilmezse session'daki kullanıcı yazılır (login sırasında henüz session yoktur).
        public static void Audit(string eventName, string detail = null, string userName = null)
        {
            Write(auditLogger, LogEventLevel.Information, userName, Compose(eventName, detail), null, eventName);
        }

        public static void AuditWarn(string eventName, string detail = null, string userName = null)
        {
            Write(auditLogger, LogEventLevel.Warning, userName, Compose(eventName, detail), null, eventName);
        }

        private static ILogger Create(string pathFormat, string logType)
        {
            LoggerConfiguration configuration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.WithProperty("Application", "TwoStepEncryptionDdms")
                .Enrich.WithProperty("LogType", logType)
                .WriteTo.File(pathFormat,
                    outputTemplate: Template,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: RetainedDays,
                    shared: true);

            // SEQ_URL doluysa log'lar ayrıca Seq'e gönderilir; server'a ulaşılamazsa uygulama etkilenmez, txt dosyasına yazılır.
            if (!string.IsNullOrWhiteSpace(AppConfig.SeqUrl))
            {
                configuration = configuration.WriteTo.Seq(AppConfig.SeqUrl);
            }

            return configuration.CreateLogger();
        }

        private static string Describe(Exception ex, string context)
        {
            string text = ex == null ? "" : ex.GetType().Name + ": " + ex.Message;
            return string.IsNullOrWhiteSpace(context) ? Clean(text) : Clean(context) + " | " + Clean(text);
        }

        private static string Compose(string eventName, string detail)
        {
            return string.IsNullOrWhiteSpace(detail) ? eventName : eventName + " | " + Clean(detail);
        }

        private static void Write(ILogger logger, LogEventLevel level, string userName, string message, Exception ex,
            string eventName = null, string errorCode = null)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                string user = Clean(userName);
                string ip = "-";
                string page = "-";

                if (context != null)
                {
                    if (string.IsNullOrEmpty(user) && context.User != null && context.User.Identity.IsAuthenticated)
                    {
                        user = context.User.Identity.Name;
                    }

                    HttpRequest request = SafeRequest(context);
                    if (request != null)
                    {
                        ip = request.UserHostAddress;
                        page = request.Url.AbsolutePath;
                    }
                }

                ILogger entry = logger
                    .ForContext("User", string.IsNullOrEmpty(user) ? "-" : user)
                    .ForContext("Ip", ip)
                    .ForContext("Page", page);

                if (eventName != null) { entry = entry.ForContext("Event", eventName); }
                if (errorCode != null) { entry = entry.ForContext("ErrorCode", errorCode); }

                entry.Write(level, ex, "{Msg:l}", message);
            }
            catch (Exception logEx)
            {
                Trace.TraceError("Log yazılamadı: " + logEx.Message);
            }
        }

        private static HttpRequest SafeRequest(HttpContext context)
        {
            try
            {
                return context.Request;
            }
            catch (HttpException)
            {
                return null;
            }
        }

        // Kullanıcıdan gelen metinler tek satırda kalır (log injection'ı önler).
        private static string Clean(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            string singleLine = text.Replace('\r', ' ').Replace('\n', ' ');
            return singleLine.Length > MaxTextLength ? singleLine.Substring(0, MaxTextLength) : singleLine;
        }
    }
}
