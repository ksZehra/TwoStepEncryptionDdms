using System;
using System.Data;
using System.Text.RegularExpressions;
using TwoStepEncryptionDdms.Data;

namespace TwoStepEncryptionDdms.Security
{
    public static class SqlIdentifier
    {
        private static readonly Regex Safe = new Regex(@"^[\p{L}\p{N}_]{1,128}$", RegexOptions.Compiled);

        public static bool IsValid(string name)
        {
            return name != null && Safe.IsMatch(name);
        }

        public static string QuoteSchemaName(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }

        public static string EscapeLiteral(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        public static string Quote(string name)
        {
            if (!IsValid(name))
            {
                throw new ArgumentException("Geçersiz nesne adı.");
            }
            return "[" + name + "]";
        }

        // System table'larına (TBL_USERS vb.) bu method ile erişilemez; table TBL_USERAUTHORIZATIONS'ta kayıtlı olmalıdır.
        public static string QuoteRegisteredTable(DataIslemleri data, string tableName)
        {
            string quoted = Quote(tableName);

            data.AddParameter("@TBLAD", tableName);
            object found = data.QueryScalar("SELECT COUNT(1) FROM TBL_USERAUTHORIZATIONS WHERE TBLAD=@TBLAD");
            if (found == null || Convert.ToInt32(found) == 0)
            {
                throw new ArgumentException("Veri kümesi bulunamadı.");
            }
            return quoted;
        }
    }
}
