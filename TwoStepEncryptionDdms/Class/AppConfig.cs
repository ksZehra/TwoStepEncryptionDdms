using System;
using System.Collections.Generic;
using System.Configuration;

namespace TwoStepEncryptionDdms
{
    // Aes256Encrypter.cs aynı key'leri kendi içinde okur; bu sınıf onun yerine geçmez.
    public static class AppConfig
    {
        // Aes256Encrypter.cs'in tanıdığı ECRYPTTYPE değerleri ("2" ayrı bir dal olarak mevcut, ENCRYPTMODE ile çalışır).
        private static readonly string[] AllowedEncryptTypes = { "ECB", "CBC", "CFB", "2" };

        private static readonly string[] Required =
        {
            "SQL_AUTH", "SQL_SERVER", "SQL_DATABASE", "DBKEY", "DBKEYPWD",
            "ECRYPTTYPE", "ISBKENCRYPT", "ISDBENCRYPT"
        };


        public static string EncryptType
        {
            get { return Get("ECRYPTTYPE"); }
        }

        public static bool IsBackendEncryptionEnabled
        {
            get { return Get("ISBKENCRYPT") == "1"; }
        }

        public static bool IsDatabaseEncryptionEnabled
        {
            get { return Get("ISDBENCRYPT") == "1"; }
        }


        public static string DbKeyName
        {
            get { return Get("DBKEY"); }
        }

        public static string DbKeyPassword
        {
            get { return Get("DBKEYPWD"); }
        }

        public static string SeqUrl
        {
            get { return Get("SEQ_URL") ?? ""; }
        }


        public static string SqlAuth
        {
            get { return Get("SQL_AUTH"); }
        }

        public static string SqlServer
        {
            get { return Get("SQL_SERVER"); }
        }

        public static string SqlDatabase
        {
            get { return Get("SQL_DATABASE"); }
        }

        public static string SqlUser
        {
            get { return Get("SQL_USER"); }
        }

        public static string SqlPassword
        {
            get { return Get("SQL_PASSWORD") ?? ""; }
        }

        public static string SqlPort
        {
            get { return Get("SQL_PORT") ?? ""; }
        }

        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void Validate()
        {
            var missing = new List<string>();
            foreach (string key in Required)
            {
                if (string.IsNullOrWhiteSpace(Get(key)))
                {
                    missing.Add(key);
                }
            }

            if (missing.Count > 0)
            {
                throw new InvalidOperationException(
                    "Eksik uygulama ayarı: " + string.Join(", ", missing) + ". " +
                    "Şifreleme ve bağlantı ayarları Web.config içinde, DBKEY/DBKEYPWD ise TwoStepEncryptionDdms klasöründeki " +
                    "appSettings.local.config dosyasında bulunmalıdır. " +
                    "Local config dosyasını Database\\setup.ps1 ile oluşturabilir veya appSettings.example.config dosyasını kopyalayıp doldurabilirsiniz.");
            }

            var invalid = new List<string>();

            if (Array.IndexOf(AllowedEncryptTypes, EncryptType) < 0)
            {
                invalid.Add("ECRYPTTYPE='" + EncryptType + "' (izin verilen değerler: " + string.Join(", ", AllowedEncryptTypes) + ")");
            }

            if (!IsFlag(Get("ISBKENCRYPT")))
            {
                invalid.Add("ISBKENCRYPT='" + Get("ISBKENCRYPT") + "' (yalnızca 0 veya 1)");
            }

            if (!IsFlag(Get("ISDBENCRYPT")))
            {
                invalid.Add("ISDBENCRYPT='" + Get("ISDBENCRYPT") + "' (yalnızca 0 veya 1)");
            }

            if (invalid.Count > 0)
            {
                throw new InvalidOperationException(
                    "Geçersiz uygulama ayarı: " + string.Join("; ", invalid) + ". Web.config içindeki değerleri düzeltin.");
            }
        }

        private static bool IsFlag(string value)
        {
            return value == "0" || value == "1";
        }
    }
}
