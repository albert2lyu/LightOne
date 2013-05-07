using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Configuration;

namespace Common.Data
{
    public class ConnectionDatabase : IDatabase {
        private readonly string _ConnectionString;
        private readonly DbProviderFactory _ProviderFactory;

        public ConnectionDatabase(string connectionStringName) {
            // 此处是否应该采用多例模式，缓存 providerFactory？
            // 经测试，5秒内可创建20万个Database对象，不必进行缓存
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            var connectionString = connectionStringSettings.ConnectionString;
            _ConnectionString = connectionString;
            _ProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
        }

        public ConnectionDatabase(string connectionString, string providerName) {
            _ConnectionString = connectionString;
            _ProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        private DbConnection CreateConnection() {
            var connection = _ProviderFactory.CreateConnection();
            connection.ConnectionString = _ConnectionString;
            connection.Open();
            return connection;
        }

        public int ExecuteNonQuery(string sql, object parameters, int commandTimeout) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    return new CommandDatabase(cmd).ExecuteNonQuery(sql, parameters, commandTimeout);
                }
            }
        }

        public int ExecuteNonQuery(string sql, IDictionary<string, object> parameters, int commandTimeout) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    return new CommandDatabase(cmd).ExecuteNonQuery(sql, parameters, commandTimeout);
                }
            }
        }

        public IEnumerable<T> ExecuteDataReader<T>(string sql, object parameters, Func<DbDataReader, T> action, int commandTimeout) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    // 这里一定要用yield，这样可以延迟执行，直接用return db.ExecuteDataReader(sql, parameters, action)在执行dr.Read()的时候，cmd对象早就释放掉了
                    foreach (var r in db.ExecuteDataReader(sql, parameters, action, commandTimeout))
                        yield return r;
                }
            }
        }

        public void ExecuteDataReader(string sql, object parameters, Action<DbDataReader> action, int commandTimeout) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    db.ExecuteDataReader(sql, parameters, action, commandTimeout);
                }
            }
        }

        //public void ExecuteTransaction(Action<DbCommand> action) {
        //    using (var connection = CreateConnection()) {
        //        using (var transaction = connection.BeginTransaction()) {
        //            try {
        //                using (var cmd = connection.CreateCommand()) {
        //                    cmd.Transaction = transaction;
        //                    cmd.Connection = connection;

        //                    if (action != null)
        //                        action.Invoke(cmd);
        //                }

        //                transaction.Commit();
        //            }
        //            catch {
        //                transaction.Rollback();
        //                throw;
        //            }
        //        }
        //    }
        //}

        public void ExecuteTransaction(IsolationLevel isolationLevel, Action<IDatabase> action) {
            ExecuteTransaction((IsolationLevel?)isolationLevel, action);
        }

        public void ExecuteTransaction(Action<IDatabase> action) {
            ExecuteTransaction(null, action);
        }

        private void ExecuteTransaction(IsolationLevel? isolationLevel, Action<IDatabase> action) {
            using (var connection = CreateConnection()) {
                using (var transaction = isolationLevel.HasValue ?
                    connection.BeginTransaction(isolationLevel.Value) :
                    connection.BeginTransaction()) {
                    try {
                        using (var cmd = connection.CreateCommand()) {
                            cmd.Transaction = transaction;

                            var db = new CommandDatabase(cmd);
                            db.ExecuteTransaction(action);
                        }

                        transaction.Commit();
                    }
                    catch {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public IDictionary<string, object> ExecuteStoredProcedure(string sql, object parameters, IDictionary<string, int> outputParameters) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    return db.ExecuteStoredProcedure(sql, parameters, outputParameters);
                }
            }
        }

        public void BulkCopy(DataTable table) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    db.BulkCopy(table);
                }
            }
        }

        public bool HasRow(string sql, object parameters) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    return db.HasRow(sql, parameters);
                }
            }
        }

        public override string ToString() {
            return _ConnectionString;
        }

        public T ExecuteScalar<T>(string sql, object parameters, int commandTimeout) {
            using (var connection = CreateConnection()) {
                using (var cmd = connection.CreateCommand()) {
                    var db = new CommandDatabase(cmd);
                    return db.ExecuteScalar<T>(sql, parameters, commandTimeout);
                }
            }
        }
    }
}
