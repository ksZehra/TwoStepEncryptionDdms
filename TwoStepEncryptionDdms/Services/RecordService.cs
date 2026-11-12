using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Repositories;
using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Services
{
    public class RecordService
    {
        private static readonly CultureInfo TurkishCulture = new CultureInfo("tr-TR");

        public void EnsureRegistered(string tblAd)
        {
            using (var data = Open())
            {
                new DatasetRepository(data).QuoteRegisteredTable(tblAd);
            }
        }

        public bool HasKey(string tblAd)
        {
            using (var data = Open())
            {
                return TableKeyService.HasKey(data, tblAd);
            }
        }

        public bool VerifyKey(HttpContext context, string tblAd, string key)
        {
            using (var data = Open())
            {
                new DatasetRepository(data).QuoteRegisteredTable(tblAd);
                return TableKeyService.Verify(context, data, tblAd, key);
            }
        }

        public RecordList GetRecords(string tblAd, bool tableHasKey, string key, string searchColumn, string searchTerm)
        {
            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                var records = new RecordRepository(data);

                string quotedTable = datasets.QuoteRegisteredTable(tblAd);

                var encrypted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (tableHasKey)
                {
                    foreach (DataRow row in datasets.GetEncryptedColumns(tblAd).Rows)
                    {
                        encrypted.Add(Convert.ToString(row["KOLONADI"]));
                    }
                }

                DataTable columnTable = datasets.GetColumns(tblAd);

                string searchTarget = null;
                if (!string.IsNullOrWhiteSpace(searchColumn))
                {
                    foreach (DataRow row in columnTable.Rows)
                    {
                        string name = Convert.ToString(row["COLUMN_NAME"]);
                        if (string.Equals(name, searchColumn, StringComparison.OrdinalIgnoreCase))
                        {
                            searchTarget = name;
                            break;
                        }
                    }

                    if (searchTarget == null)
                    {
                        throw new UserException("Belirtilen sütun adı bu veri kümesinde bulunamadı!");
                    }
                }

                var columns = new List<RecordColumn>();
                var selectList = new List<string>();
                foreach (DataRow row in columnTable.Rows)
                {
                    string name = Convert.ToString(row["COLUMN_NAME"]);
                    bool isEncrypted = encrypted.Contains(name);
                    columns.Add(new RecordColumn(name, isEncrypted));
                    selectList.Add(EncryptedSql.SelectColumn(name, isEncrypted));
                }

                string whereSql = null;
                object whereValue = null;
                int memorySearchIndex = -1;
                if (searchTarget != null && !string.IsNullOrWhiteSpace(searchTerm))
                {
                    if (!datasets.IsEncryptedColumn(tblAd, searchTarget))
                    {
                        whereSql = SqlIdentifier.QuoteSchemaName(searchTarget) + " LIKE @ARANAN ESCAPE '\\' ";
                        whereValue = "%" + SqlLike.Escape(searchTerm) + "%";
                    }
                    else
                    {
                        // Encryption her seferinde farklı output ürettiği (random IV) için encrypted column database'de aranamaz; decrypt edilen değerler memory'de filtrelenir.
                        memorySearchIndex = columns.FindIndex(c => string.Equals(c.Name, searchTarget, StringComparison.OrdinalIgnoreCase));
                    }
                }

                DataTable table = records.Select(quotedTable, string.Join(",", selectList), whereSql, whereValue);

                bool decrypt = AppConfig.IsBackendEncryptionEnabled;
                var rows = new List<string[]>();
                foreach (DataRow row in table.Rows)
                {
                    var cells = new string[columns.Count];
                    for (int j = 0; j < cells.Length; j++)
                    {
                        string value = row[j].ToString();
                        cells[j] = columns[j].IsEncrypted && decrypt ? Aes256Encrypter.Decrypt(value, key) : value;
                    }

                    if (memorySearchIndex >= 0 && !ContainsIgnoreCase(cells[memorySearchIndex], searchTerm))
                    {
                        continue;
                    }

                    rows.Add(cells);
                }

                return new RecordList(columns, rows);
            }
        }

        private static bool ContainsIgnoreCase(string text, string term)
        {
            return TurkishCulture.CompareInfo.IndexOf(text ?? "", term, CompareOptions.IgnoreCase) >= 0;
        }

        public void DeleteRecord(string tblAd, int recordId)
        {
            using (var data = Open())
            {
                string quotedTable = new DatasetRepository(data).QuoteRegisteredTable(tblAd);
                new RecordRepository(data).Delete(quotedTable, recordId);
            }
        }

        public IList<EditField> GetEditFields(string tblAd, string recordId, string key)
        {
            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                string quotedTable = datasets.QuoteRegisteredTable(tblAd);

                DataTable columnTable = datasets.GetColumns(tblAd);
                DataTable encryptedTable = datasets.GetEncryptedColumns(tblAd);

                var encrypted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow row in encryptedTable.Rows)
                {
                    encrypted.Add(Convert.ToString(row["KOLONADI"]));
                }

                var fields = new List<EditField>();
                var selectList = new List<string>();
                for (int j = 1; j < columnTable.Rows.Count; j++)
                {
                    string name = columnTable.Rows[j]["COLUMN_NAME"].ToString();
                    bool isEncrypted = encrypted.Contains(name);
                    fields.Add(new EditField(name, isEncrypted));
                    selectList.Add(EncryptedSql.SelectColumn(name, isEncrypted));
                }

                if (!string.IsNullOrWhiteSpace(recordId) && fields.Count > 0)
                {
                    DataTable record = new RecordRepository(data).SelectById(quotedTable, string.Join(",", selectList), recordId);
                    bool decrypt = AppConfig.IsBackendEncryptionEnabled;
                    for (int k = 0; k < fields.Count; k++)
                    {
                        string value = record.Rows.Count > 0 ? Convert.ToString(record.Rows[0][k]) : "";
                        fields[k].Value = fields[k].IsEncrypted && decrypt ? Aes256Encrypter.Decrypt(value, key, "") : value;
                    }
                }

                return fields;
            }
        }

        public void SaveRecord(HttpContext context, string tblAd, string recordId, string key, IList<KeyValuePair<string, string>> postedValues)
        {
            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                var records = new RecordRepository(data);

                string quotedTable = datasets.QuoteRegisteredTable(tblAd);
                data.BeginTx();
                try
                {
                    DataTable encryptedTable = datasets.GetEncryptedColumns(tblAd);

                    // Key server-side verify edilmeden encrypted column'lara kayıt yazılamaz.
                    if (encryptedTable.Rows.Count > 0 && !TableKeyService.Verify(context, data, tblAd, key))
                    {
                        throw new UserException("Anahtar bilgisi yanlış! Lütfen tekrar deneyiniz!");
                    }

                    var encrypted = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRow row in encryptedTable.Rows)
                    {
                        encrypted.Add(Convert.ToString(row["KOLONADI"]));
                    }

                    // Client'tan gelen column adları table'ın gerçek column'larıyla eşleştirilir (KAYITID hariç).
                    var realColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRow row in datasets.GetColumnsExceptId(tblAd).Rows)
                    {
                        string name = Convert.ToString(row["COLUMN_NAME"]);
                        realColumns[name] = name;
                    }

                    bool encryptBackend = AppConfig.IsBackendEncryptionEnabled;
                    var columnList = new List<string>();
                    var valueList = new List<string>();
                    var assignmentList = new List<string>();
                    var parameters = new List<KeyValuePair<string, string>>();
                    string allValues = "";
                    int counter = 0;

                    foreach (var posted in postedValues)
                    {
                        string realColumn;
                        if (!realColumns.TryGetValue(posted.Key, out realColumn))
                        {
                            throw new UserException("Geçersiz kolon bilgisi!");
                        }

                        string quotedColumn = SqlIdentifier.QuoteSchemaName(realColumn);
                        string parameterName = "@p" + counter;
                        counter++;

                        bool isEncrypted = encrypted.Contains(realColumn);
                        string valueToSave = posted.Value;
                        if (isEncrypted && encryptBackend)
                        {
                            valueToSave = Aes256Encrypter.Encrypt(posted.Value, key);
                        }

                        string expression = EncryptedSql.WriteValue(parameterName, isEncrypted);
                        parameters.Add(new KeyValuePair<string, string>(parameterName, valueToSave));
                        columnList.Add(quotedColumn);
                        valueList.Add(expression);
                        assignmentList.Add(quotedColumn + "=" + expression);
                        allValues += posted.Value;
                    }

                    if (columnList.Count == 0 || string.IsNullOrWhiteSpace(allValues))
                    {
                        throw new UserException("Boş kayıt eklenemez!");
                    }

                    if (!string.IsNullOrWhiteSpace(recordId))
                    {
                        records.Update(quotedTable, assignmentList, parameters, recordId);
                    }
                    else
                    {
                        records.Insert(quotedTable, columnList, valueList, parameters);
                    }

                    data.CommitTx();
                }
                catch
                {
                    data.RollbackTx();
                    throw;
                }
            }
        }

        private static DataIslemleri Open()
        {
            var data = new DataIslemleri();
            data.Open();
            return data;
        }
    }
}
