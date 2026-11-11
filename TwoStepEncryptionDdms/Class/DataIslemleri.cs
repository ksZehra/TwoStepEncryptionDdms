using System;
using System.Data;
using System.Data.SqlClient;

namespace TwoStepEncryptionDdms.Data
{
    public class DataIslemleri : IDisposable
    {
        private const int CommandTimeoutSeconds = 60;

        private readonly string connectionString;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlTransaction transaction;

        public DataIslemleri()
        {
            var builder = new SqlConnectionStringBuilder { InitialCatalog = AppConfig.SqlDatabase };

            string port = (AppConfig.SqlPort ?? "").Trim();
            builder.DataSource = port.Length > 0 ? AppConfig.SqlServer + "," + port : AppConfig.SqlServer;

            if (AppConfig.SqlAuth == "Trusted")
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = AppConfig.SqlUser;
                builder.Password = AppConfig.SqlPassword;
            }

            connectionString = builder.ConnectionString;
        }

        public void Open()
        {
            Dispose();

            var newConnection = new SqlConnection(connectionString);
            try
            {
                newConnection.Open();
                connection = newConnection;
                command = newConnection.CreateCommand();
                command.CommandTimeout = CommandTimeoutSeconds;
            }
            catch
            {
                newConnection.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            // Tamamlanmamış transaction Dispose ile geri alınır.
            if (transaction != null) { transaction.Dispose(); transaction = null; }
            if (command != null) { command.Dispose(); command = null; }
            if (connection != null) { connection.Dispose(); connection = null; }
        }

        public void BeginTx()
        {
            transaction = connection.BeginTransaction();
            command.Transaction = transaction;
        }

        public void CommitTx()
        {
            if (transaction != null && transaction.Connection != null)
            {
                transaction.Commit();
            }
        }

        public void RollbackTx()
        {
            if (transaction != null && transaction.Connection != null)
            {
                transaction.Rollback();
            }
        }

        // null, boş string ve sıfır tarih DBNull olarak gönderilir; encrypted field'a boş değer girilirse NULL kaydedilir.
        public void AddParameter(string name, object value)
        {
            if (value == null
                || (value is string && string.IsNullOrWhiteSpace((string)value))
                || (value is DateTime && (DateTime)value == DateTime.MinValue))
            {
                value = DBNull.Value;
            }
            command.Parameters.Add(new SqlParameter(name, value));
        }

        public DataTable QueryTable(string sql)
        {
            return Run(() =>
            {
                var table = new DataTable();
                using (var adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(table);
                }
                return table;
            }, sql);
        }

        public object QueryScalar(string sql)
        {
            return Run(() => command.ExecuteScalar(), sql);
        }

        public void Execute(string sql)
        {
            Run(() => command.ExecuteNonQuery(), sql);
        }

        private T Run<T>(Func<T> action, string sql)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sql;
                return action();
            }
            finally
            {
                command.Parameters.Clear();
            }
        }
    }
}
