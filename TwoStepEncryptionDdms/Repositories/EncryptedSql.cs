using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Repositories
{
    internal static class EncryptedSql
    {
        // RSA_2048 ile tek seferde en fazla 190 byte encrypt edilebilir; fazlasında ENCRYPTBYASYMKEY hata vermeden NULL döner.
        public const int MaxRsaPayloadBytes = 190;

        public const string KeyPasswordParameter = "@DBKEYPWD";

        public static string SelectColumn(string columnName, bool encrypted)
        {
            string quoted = SqlIdentifier.QuoteSchemaName(columnName);
            return encrypted && AppConfig.IsDatabaseEncryptionEnabled ? Decrypt(quoted) : quoted;
        }

        public static string WriteValue(string parameterName, bool encrypted)
        {
            return encrypted && AppConfig.IsDatabaseEncryptionEnabled ? Encrypt(parameterName) : parameterName;
        }

        private static string Decrypt(string quotedColumn)
        {
            return "CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('" + KeyName + "')," + quotedColumn + "," + KeyPasswordParameter + "))";
        }

        private static string Encrypt(string parameterName)
        {
            return "ENCRYPTBYASYMKEY(ASYMKEY_ID('" + KeyName + "'),CONVERT(VARCHAR(600)," + parameterName + "))";
        }

        private static string KeyName
        {
            get { return SqlIdentifier.EscapeLiteral(AppConfig.DbKeyName); }
        }
    }
}
