using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Repositories
{
    internal static class EncryptedSql
    {
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
            return "CONVERT(VARCHAR(600),DECRYPTBYASYMKEY(ASYMKEY_ID('" + KeyName + "')," + quotedColumn + ",N'" + KeyPassword + "'))";
        }

        private static string Encrypt(string parameterName)
        {
            return "ENCRYPTBYASYMKEY(ASYMKEY_ID('" + KeyName + "'),CONVERT(VARCHAR(600)," + parameterName + "))";
        }

        private static string KeyName
        {
            get { return SqlIdentifier.EscapeLiteral(AppConfig.DbKeyName); }
        }

        private static string KeyPassword
        {
            get { return SqlIdentifier.EscapeLiteral(AppConfig.DbKeyPassword); }
        }
    }
}
