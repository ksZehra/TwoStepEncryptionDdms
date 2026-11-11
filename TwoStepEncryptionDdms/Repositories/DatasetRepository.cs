using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Security;
using TwoStepEncryptionDdms.Services;

namespace TwoStepEncryptionDdms.Repositories
{
    public class DatasetRepository
    {
        private readonly DataIslemleri _data;

        public DatasetRepository(DataIslemleri data)
        {
            _data = data;
        }

        // Kullanıcı veri kümeleri ddms schema'sında tutulur; system table'ları dbo'da kalır.
        public const string SchemaName = "ddms";

        public static string TableReference(string quotedTable)
        {
            return SchemaName + "." + quotedTable;
        }

        public string QuoteRegisteredTable(string tblAd)
        {
            return SqlIdentifier.QuoteRegisteredTable(_data, tblAd);
        }

        public DataTable GetColumns(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            return _data.QueryTable(
                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" + SchemaName + "' AND TABLE_NAME =@TBLAD ORDER BY ORDINAL_POSITION");
        }

        public DataTable GetColumnsExceptId(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            return _data.QueryTable(
                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" + SchemaName + "' AND TABLE_NAME=@TBLAD AND COLUMN_NAME<>'KAYITID'");
        }

        public DataTable GetEncryptedColumns(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            return _data.QueryTable("SELECT KOLONADI FROM TBL_SIFRELIKOLONLAR WHERE TBLAD=@TBLAD");
        }

        public bool IsEncryptedColumn(string tblAd, string columnName)
        {
            _data.AddParameter("@TBLAD", tblAd);
            _data.AddParameter("@KOLONADI", columnName);
            return System.Convert.ToInt32(_data.QueryScalar(
                "SELECT COUNT(ID) FROM TBL_SIFRELIKOLONLAR WHERE TBLAD=@TBLAD AND KOLONADI=@KOLONADI")) > 0;
        }

        public DataTable List(string search)
        {
            string sql = @"WITH SIFRELIMI AS(
                                SELECT TBLAD FROM TBL_SIFRELIKOLONLAR
                                GROUP BY TBLAD
                           )
                           SELECT A.TBLID,A.TBLAD,
                           CASE WHEN B.TBLAD IS NOT NULL THEN 'Evet' ELSE 'Hayır' END ISENCRYPT,
                           A.OLUSTURAN,CONVERT(VARCHAR(10),A.OLUSTURMATARIHI,104) OLUSTURMATARIHI,A.GUNCELLEYEN,CONVERT(VARCHAR(10),A.GUNCELLEMETARIHI,104) GUNCELLEMETARIHI
                           FROM TBL_USERAUTHORIZATIONS A
                           LEFT JOIN SIFRELIMI B ON B.TBLAD=A.TBLAD ";
            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += @" WHERE A.TBLAD LIKE @ARA ESCAPE '\' ";
                _data.AddParameter("@ARA", "%" + SqlLike.Escape(search.Trim()) + "%");
            }
            sql += " ORDER BY A.TBLID DESC";
            return _data.QueryTable(sql);
        }

        public bool TableExists(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            return _data.QueryScalar("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='" + SchemaName + "' AND TABLE_NAME = @TBLAD") != null;
        }

        public void CreateTable(string tblAd, IList<string> columnNames)
        {
            var columns = new StringBuilder("KAYITID INT IDENTITY(1,1) NOT NULL");
            foreach (string name in columnNames)
            {
                columns.Append(',').Append(SqlIdentifier.Quote(name)).Append(" VARCHAR(600)");
            }
            _data.Execute("CREATE TABLE " + TableReference(SqlIdentifier.Quote(tblAd)) + "(" + columns + ");");
        }

        public void AddEncryptedColumn(string tblAd, string columnName)
        {
            _data.AddParameter("@TBLAD", tblAd);
            _data.AddParameter("@KOLONADI", columnName);
            _data.Execute("INSERT INTO TBL_SIFRELIKOLONLAR(TBLAD,KOLONADI) VALUES(@TBLAD,@KOLONADI);");
        }

        public void InsertDataset(string tblAd, string keyVerifier, int userId, string fullName)
        {
            _data.AddParameter("@TBLAD", tblAd);
            _data.AddParameter("@KEYVERIFIER", keyVerifier);
            _data.AddParameter("@OLUSTURANID", userId);
            _data.AddParameter("@OLUSTURAN", fullName);
            _data.AddParameter("@OLUSTURMATARIHI", System.DateTime.Now);
            _data.Execute(
                @"INSERT INTO TBL_USERAUTHORIZATIONS (TBLAD,KEYVERIFIER,OLUSTURANID,OLUSTURAN,OLUSTURMATARIHI)
                  VALUES (@TBLAD,@KEYVERIFIER,@OLUSTURANID,@OLUSTURAN,@OLUSTURMATARIHI)");
        }

        public void DropTable(string quotedTable)
        {
            _data.Execute("DROP TABLE " + TableReference(quotedTable));
        }

        public void DeleteDatasetRecord(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            _data.Execute("DELETE FROM TBL_USERAUTHORIZATIONS WHERE TBLAD=@TBLAD");
        }

        public void DeleteEncryptedColumns(string tblAd)
        {
            _data.AddParameter("@TBLAD", tblAd);
            _data.Execute("DELETE FROM TBL_SIFRELIKOLONLAR WHERE TBLAD=@TBLAD");
        }

        public void RenameDatasetRecord(string oldName, string newName, int userId, string fullName)
        {
            _data.AddParameter("@NEWAD", newName);
            _data.AddParameter("@OLDAD", oldName);
            _data.AddParameter("@GUNCELLEYENID", userId);
            _data.AddParameter("@GUNCELLEYEN", fullName);
            _data.AddParameter("@GUNCELLEMETARIHI", System.DateTime.Now);
            _data.Execute(
                @"UPDATE TBL_USERAUTHORIZATIONS
                  SET TBLAD =@NEWAD,GUNCELLEYENID=@GUNCELLEYENID,GUNCELLEYEN=@GUNCELLEYEN,GUNCELLEMETARIHI=@GUNCELLEMETARIHI
                  WHERE TBLAD =@OLDAD");
        }

        public void RenameEncryptedColumns(string oldName, string newName)
        {
            _data.AddParameter("@YENITBLAD", newName);
            _data.AddParameter("@ESKITBLAD", oldName);
            _data.Execute("UPDATE TBL_SIFRELIKOLONLAR SET TBLAD=@YENITBLAD WHERE TBLAD=@ESKITBLAD");
        }

        public void RenameTable(string oldName, string newName)
        {
            _data.AddParameter("@newAd", newName);
            _data.AddParameter("@oldAd", SchemaName + "." + oldName);
            _data.Execute("EXEC SP_RENAME @oldAd,@newAd");
        }

        public DashboardSummary GetDashboardSummary()
        {
            DataTable table = _data.QueryTable(@"
                SELECT
                    (SELECT COUNT(*) FROM TBL_USERAUTHORIZATIONS) AS DatasetCount,
                    (SELECT COUNT(DISTINCT TBLAD) FROM TBL_SIFRELIKOLONLAR) AS EncryptedDatasetCount,
                    ISNULL((
                        SELECT SUM(CONVERT(BIGINT, p.rows))
                        FROM sys.tables t
                        INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
                        INNER JOIN sys.partitions p ON p.object_id = t.object_id AND p.index_id IN (0,1)
                        WHERE s.name = 'ddms'
                          AND t.name IN (SELECT TBLAD FROM TBL_USERAUTHORIZATIONS)
                    ), 0) AS RecordCount;");

            var summary = new DashboardSummary();
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                summary.DatasetCount = Convert.ToInt32(row["DatasetCount"]);
                summary.EncryptedDatasetCount = Convert.ToInt32(row["EncryptedDatasetCount"]);
                summary.RecordCount = Convert.ToInt64(row["RecordCount"]);
            }
            return summary;
        }
    }
}
