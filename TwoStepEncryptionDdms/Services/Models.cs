using System.Collections.Generic;

namespace TwoStepEncryptionDdms.Services
{
    public sealed class DatasetColumn
    {
        public DatasetColumn(string name, bool isEncrypted)
        {
            Name = name;
            IsEncrypted = isEncrypted;
        }

        public string Name { get; private set; }
        public bool IsEncrypted { get; private set; }
    }

    public sealed class RecordColumn
    {
        public RecordColumn(string name, bool isEncrypted)
        {
            Name = name;
            IsEncrypted = isEncrypted;
        }

        public string Name { get; private set; }
        public bool IsEncrypted { get; private set; }
    }

    // Row'lardaki encrypted cell'ler decrypt edilmiş halde gelir.
    public sealed class RecordList
    {
        public RecordList(IList<RecordColumn> columns, IList<string[]> rows)
        {
            Columns = columns;
            Rows = rows;
        }

        public IList<RecordColumn> Columns { get; private set; }
        public IList<string[]> Rows { get; private set; }
    }

    public sealed class EditField
    {
        public EditField(string name, bool isEncrypted)
        {
            Name = name;
            IsEncrypted = isEncrypted;
            Value = "";
        }

        public string Name { get; private set; }
        public bool IsEncrypted { get; private set; }
        public string Value { get; set; }
    }

    public sealed class AuthenticatedUser
    {
        public AuthenticatedUser(int id, string userName, string fullName)
        {
            Id = id;
            UserName = userName;
            FullName = fullName;
        }

        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string FullName { get; private set; }
    }

    public sealed class DashboardSummary
    {
        public int DatasetCount { get; set; }
        public int EncryptedDatasetCount { get; set; }
        public long RecordCount { get; set; }
    }
}
