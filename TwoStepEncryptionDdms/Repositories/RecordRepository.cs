using System.Collections.Generic;
using System.Data;
using TwoStepEncryptionDdms.Data;

namespace TwoStepEncryptionDdms.Repositories
{
    public class RecordRepository
    {
        private readonly DataIslemleri _data;

        public RecordRepository(DataIslemleri data)
        {
            _data = data;
        }

        public DataTable Select(string quotedTable, string selectList, string whereSql, object whereValue)
        {
            string sql = "SELECT " + selectList + " FROM " + DatasetRepository.TableReference(quotedTable);
            if (whereSql != null)
            {
                sql += " WHERE " + whereSql;
                _data.AddParameter("@ARANAN", whereValue);
            }
            return _data.QueryTable(sql);
        }

        public DataTable SelectById(string quotedTable, string selectList, string recordId)
        {
            _data.AddParameter("@KAYITID", recordId);
            return _data.QueryTable(
                "SELECT " + selectList + " FROM " + DatasetRepository.TableReference(quotedTable) + " WHERE KAYITID=@KAYITID");
        }

        public void Delete(string quotedTable, int recordId)
        {
            _data.AddParameter("@KAYITID", recordId);
            _data.QueryScalar("DELETE FROM " + DatasetRepository.TableReference(quotedTable) + " WHERE KAYITID=@KAYITID");
        }

        public void Insert(string quotedTable, IList<string> columns, IList<string> valueExpressions, IList<KeyValuePair<string, string>> parameters)
        {
            foreach (var parameter in parameters)
            {
                _data.AddParameter(parameter.Key, parameter.Value);
            }
            _data.Execute(
                "INSERT INTO " + DatasetRepository.TableReference(quotedTable) + " (" + string.Join(",", columns) + ") VALUES(" + string.Join(",", valueExpressions) + ");");
        }

        public void Update(string quotedTable, IList<string> assignments, IList<KeyValuePair<string, string>> parameters, string recordId)
        {
            foreach (var parameter in parameters)
            {
                _data.AddParameter(parameter.Key, parameter.Value);
            }
            _data.AddParameter("@KAYITID", recordId);
            _data.Execute(
                "UPDATE " + DatasetRepository.TableReference(quotedTable) + " SET " + string.Join(",", assignments) + " WHERE KAYITID=@KAYITID");
        }
    }
}
