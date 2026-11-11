using System;
using System.Collections.Generic;
using System.Data;
using TwoStepEncryptionDdms.Data;
using TwoStepEncryptionDdms.Repositories;
using TwoStepEncryptionDdms.Security;

namespace TwoStepEncryptionDdms.Services
{
    public class DatasetService
    {
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

            if (!SqlIdentifier.IsValid(tableName) || tableName.StartsWith("TBL_", StringComparison.OrdinalIgnoreCase))
            {
                throw new UserException("Tablo adı yalnızca harf, rakam ve alt çizgi içerebilir ve 'TBL_' ile başlayamaz!");
            }

            var names = new List<string>();
            foreach (DatasetColumn column in columns)
            {
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
                throw new UserException("Boş kolona sahip bir tablo oluşturulamaz!");
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
                        throw new UserException("Bu tablo daha önce oluşturulmuş. Lütfen yeni bir tablo adı deneyiniz!");
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
                throw new UserException("Tablo adı boş bırakılamaz!");
            }
            if (newName.Length > 120)
            {
                throw new UserException("Tablo adı 120 karakterden fazla olamaz!");
            }
            if (!SqlIdentifier.IsValid(newName))
            {
                throw new UserException("Tablo adı yalnızca harf, rakam ve alt çizgi içerebilir!");
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
