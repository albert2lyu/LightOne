using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;

namespace Common.Data {
    public interface IDatabase {
        int ExecuteNonQuery(string sql, object parameters = null, int commandTimeout = 0);

        int ExecuteNonQuery(string sql, IDictionary<string, object> parameters, int commandTimeout = 0);

        IEnumerable<T> ExecuteDataReader<T>(string sql, object parameters, Func<DbDataReader, T> action, int commandTimeout = 0);

        void ExecuteDataReader(string sql, object parameters, Action<DbDataReader> action, int commandTimeout = 0);
        
        void ExecuteTransaction(Action<IDatabase> action);

        void ExecuteTransaction(IsolationLevel isolationLevel, Action<IDatabase> action);

        IDictionary<string, object> ExecuteStoredProcedure(string sql, object parameters, IDictionary<string, int> outputParameters);

        void BulkCopy(DataTable table);

        bool HasRow(string sql, object parameters);

        T ExecuteScalar<T>(string sql, object parameters = null, int commandTimeout = 0);
    }
}
