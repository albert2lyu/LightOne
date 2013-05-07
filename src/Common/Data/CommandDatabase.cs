using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;

namespace Common.Data
{
    public class CommandDatabase : IDatabase {
        public readonly DbCommand Command;

        public CommandDatabase(DbCommand cmd) {
            Command = cmd;
        }

        private void PrepareCommand(string sql, object parameters, int commandTimeout) {
            Command.CommandType = CommandType.Text;
            Command.CommandText = sql;
            Command.SetParameters(parameters);
            if (commandTimeout > 0)
                Command.CommandTimeout = commandTimeout;
        }

        public int ExecuteNonQuery(string sql, object parameters, int commandTimeout) {
            PrepareCommand(sql, parameters, commandTimeout);

            return Command.ExecuteNonQuery();
        }

        public int ExecuteNonQuery(string sql, IDictionary<string, object> parameters, int commandTimeout) {
            PrepareCommand(sql, parameters, commandTimeout);

            return Command.ExecuteNonQuery();
        }

        public IEnumerable<T> ExecuteDataReader<T>(string sql, object parameters, Func<DbDataReader, T> action, int commandTimeout) {
            PrepareCommand(sql, parameters, commandTimeout);

            using (var dr = Command.ExecuteReader()) {
                while (dr.Read())
                    yield return action.Invoke(dr);
            }
        }

        public T ExecuteScalar<T>(string sql, object parameters, int commandTimeout) {
            PrepareCommand(sql, parameters, commandTimeout);

            return (T)Command.ExecuteScalar();
        }

        public void ExecuteDataReader(string sql, object parameters, Action<DbDataReader> action, int commandTimeout) {
            PrepareCommand(sql, parameters, commandTimeout);

            using (var dr = Command.ExecuteReader()) {
                while (dr.Read())
                    action.Invoke(dr);
            }
        }

        public void ExecuteTransaction(Action<IDatabase> action) {
            if (action != null)
                action.Invoke(this);
        }

        public void ExecuteTransaction(IsolationLevel isolationLevel, Action<IDatabase> action) {
            if (action != null)
                action.Invoke(this);
        }

        public IDictionary<string, object> ExecuteStoredProcedure(string sql, object parameters, IDictionary<string, int> outputParameters) {
            Command.CommandText = sql;
            Command.CommandType = CommandType.StoredProcedure;
            Command.SetParameters(parameters);

            Command.AppendOutParameters(outputParameters);
            Command.ExecuteNonQuery();

            var returnParameters = new Dictionary<string, object>();
            foreach (var p in Command.GetReturnParametersValue()) {
                returnParameters.Add(p.Key.TrimStart('@'), p.Value);
            }
            return returnParameters;
        }

        public void BulkCopy(DataTable table) {
            using (var bulkcopy = new SqlBulkCopy((SqlConnection)Command.Connection)) {
                if (table != null && table.Rows.Count > 0) {
                    bulkcopy.DestinationTableName = table.TableName;
                    bulkcopy.BatchSize = 100;
                    bulkcopy.WriteToServer(table);
                }
            }
        }

        public bool HasRow(string sql, object parameters) {
            Command.CommandType = CommandType.Text;
            Command.CommandText = sql;
            Command.SetParameters(parameters);

            using (var dr = Command.ExecuteReader()) {
                return dr.HasRows;
            }
        }
    }
}
