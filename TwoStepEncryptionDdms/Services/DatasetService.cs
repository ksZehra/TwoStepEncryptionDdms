using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Repositories;
using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Services
{
    public class DatasetService
    {
        public const int MaxNameLength = 70;

        public DataTable ListDatasets(string search)
        {
            using (var data = Open())
            {
                return new DatasetRepository(data).List(search);
            }
        }

        public DashboardSummary GetDashboardSummary()
        {
            using (var data = Open())
            {
                return new DatasetRepository(data).GetDashboardSummary();
            }
        }

        public void CreateDataset(int userId, string fullName, string tableName, IList<DatasetColumn> columns, string key)
        {
            int encryptedCount = 0;
            foreach (DatasetColumn column in columns)
            {
                if (column.IsEncrypted) encryptedCount++;
            }

            if (encryptedCount > 0 && string.IsNullOrWhiteSpace(key))
            {
                throw new UserException("Şifrelenmesini istediğiniz veriler olduğu için anahtar(key) bilgisini boş bırakılamaz!");
            }
            if (encryptedCount > 0 && key.Length != 32)
            {
                throw new UserException("'Anahtar(Key)' uzunluğu 32 karakter olmak zorundadır! Lütfen anahtarı 32 karakter olacak şekilde düzenleyiniz.");
            }
            if (encryptedCount > 0 && key.Any(c => c > 127))
            {
                throw new UserException("'Anahtar(Key)' yalnızca ASCII karakterlerden oluşmalıdır! Türkçe karakter (ç, ğ, ı, ö, ş, ü) kullanmayınız.");
            }

            if (!SqlIdentifier.IsValid(tableName) || tableName.StartsWith("TBL_", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserException("Veri kümesi adı yalnızca harf, rakam ve alt çizgi içerebilir; 'TBL_' ile başlayamaz.");
            }

            if (tableName.Length > MaxNameLength)
            {
                throw new UserException("Veri kümesi adı " + MaxNameLength + " karakterden fazla olamaz!");
            }

            var names = new List<string>();
            foreach (DatasetColumn column in columns)
            {
                if (column.Name != null && column.Name.Length > MaxNameLength)
                {
                    throw new UserException("Sütun adı " + MaxNameLength + " karakterden fazla olamaz!");
                }
                if (!SqlIdentifier.IsValid(column.Name) || string.Equals(column.Name, "KAYITID", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UserException("Kolon adları yalnızca harf, rakam ve alt çizgi içerebilir ('KAYITID' ayrılmıştır)!");
                }
                names.Add(column.Name);
            }

            if (new HashSet<string>(names).Count != names.Count)
            {
                throw new UserException("Aynı kolon adından birden fazla bulunmaktadır! Lütfen her bir kolon adından birer tane olacak şekilde düzenleyiniz.");
            }
            if (names.Count < 1)
            {
                throw new UserException("Boş sütun içeren bir veri kümesi oluşturulamaz.");
            }

            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                data.BeginTx();
                try
                {
                    foreach (DatasetColumn column in columns)
                    {
                        if (column.IsEncrypted) datasets.AddEncryptedColumn(tableName, column.Name);
                    }

                    if (datasets.TableExists(tableName))
                    {
                        throw new UserException("Bu adla bir veri kümesi zaten var. Lütfen farklı bir ad deneyin.");
                    }

                    datasets.CreateTable(tableName, names);

                    string verifier = encryptedCount > 0 ? TableKeyService.CreateVerifier(key) : "";
                    datasets.InsertDataset(tableName, verifier, userId, fullName);

                    data.CommitTx();
                }
                catch
                {
                    data.RollbackTx();
                    throw;
                }
            }
        }

        public void DeleteDataset(string tableName)
        {
            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                string quotedTable = datasets.QuoteRegisteredTable(tableName);
                data.BeginTx();
                try
                {
                    datasets.DropTable(quotedTable);
                    datasets.DeleteDatasetRecord(tableName);
                    datasets.DeleteEncryptedColumns(tableName);
                    data.CommitTx();
                }
                catch
                {
                    data.RollbackTx();
                    throw;
                }
            }
        }

        public void RenameDataset(string oldName, string newName, int userId, string fullName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new UserException("Veri kümesi adı boş bırakılamaz.");
            }
            if (newName.Length > MaxNameLength)
            {
                throw new UserException("Veri kümesi adı " + MaxNameLength + " karakterden fazla olamaz!");
            }
            if (!SqlIdentifier.IsValid(newName))
            {
                throw new UserException("Veri kümesi adı yalnızca harf, rakam ve alt çizgi içerebilir.");
            }

            using (var data = Open())
            {
                var datasets = new DatasetRepository(data);
                datasets.QuoteRegisteredTable(oldName);
                data.BeginTx();
                try
                {
                    datasets.RenameDatasetRecord(oldName, newName, userId, fullName);
                    datasets.RenameEncryptedColumns(oldName, newName);
                    datasets.RenameTable(oldName, newName);
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
