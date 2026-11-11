using System;
using System.Security.Cryptography;
using System.Text;

namespace TwoStepEncryptionDdms.Security
{
    public enum PasswordVerifyResult
    {
        Failed,
        Success,
        /// <summary>Parola doğru ama hash eski formatta (MD5) ya da eski parametrelerle üretilmiş; yeniden hash'lenmeli.</summary>
        SuccessRehashNeeded
    }

    // Storage format: PBKDF2$SHA256$iteration$salt(base64)$hash(base64). Eski MD5 hash'leri verify edilir ve ilk başarılı login'de yeni format'a taşınır.
    public static class PasswordHasher
    {
        private const string Prefix = "PBKDF2";
        private const int Iterations = 600000; // OWASP önerisi (PBKDF2-HMAC-SHA256)
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = Derive(password, salt, Iterations, HashSize);
            return string.Join("$", Prefix, "SHA256", Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public static PasswordVerifyResult Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(stored))
            {
                return PasswordVerifyResult.Failed;
            }

            if (stored.StartsWith(Prefix + "$", StringComparison.Ordinal))
            {
                string[] parts = stored.Split('$');
                int iterations;
                if (parts.Length != 5 || parts[1] != "SHA256" || !int.TryParse(parts[2], out iterations) || iterations < 1)
                {
                    return PasswordVerifyResult.Failed;
                }

                byte[] salt, expected;
                try
                {
                    salt = Convert.FromBase64String(parts[3]);
                    expected = Convert.FromBase64String(parts[4]);
                }
                catch (FormatException)
                {
                    return PasswordVerifyResult.Failed;
                }

                byte[] actual = Derive(password, salt, iterations, expected.Length);
                if (!FixedTimeEquals(actual, expected))
                {
                    return PasswordVerifyResult.Failed;
                }

                return iterations < Iterations ? PasswordVerifyResult.SuccessRehashNeeded : PasswordVerifyResult.Success;
            }

            // Legacy: salt'sız MD5 (32 karakter hex). Yalnızca eski kayıtları verify etmek için tutulur.
            if (stored.Length == 32)
            {
                string legacy = LegacyMd5(password);
                bool same = FixedTimeEquals(Encoding.ASCII.GetBytes(legacy), Encoding.ASCII.GetBytes(stored.ToLowerInvariant()));
                return same ? PasswordVerifyResult.SuccessRehashNeeded : PasswordVerifyResult.Failed;
            }

            return PasswordVerifyResult.Failed;
        }

        // Kullanıcı bulunamadığında da aynı cost'ta hesaplama yapılır (user enumeration).
        public static void BurnTime(string password)
        {
            Derive(password ?? string.Empty, new byte[SaltSize], Iterations, HashSize);
        }

        public static bool SecureEquals(string a, string b)
        {
            return FixedTimeEquals(Encoding.UTF8.GetBytes(a ?? string.Empty), Encoding.UTF8.GetBytes(b ?? string.Empty));
        }

        // Yalnızca eski MD5 kayıtlarını verify etmek için; eski kodla aynı sonucu üretmesi gerekir (Encoding.Default).
        private static string LegacyMd5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static byte[] Derive(string password, byte[] salt, int iterations, int length)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(length);
            }
        }

        // .NET Framework 4.8'de CryptographicOperations.FixedTimeEquals yok; constant-time karşılaştırma elle yazıldı.
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
