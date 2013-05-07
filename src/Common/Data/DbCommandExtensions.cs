using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Common;

namespace Common.Data
{
    public static class DbCommandExtensions {
        public static void SetParameters(this DbCommand cmd, object parameters) {
            if (parameters == null)
                return;

            if (cmd.Parameters != null)
                cmd.Parameters.Clear();

            if (parameters is IDictionary<string, object>) {
                foreach (var kvp in (IDictionary<string, object>)parameters) {
                    AddParameter(cmd, kvp.Key, kvp.Value);
                }
            }
            else {
                var t = parameters.GetType();
                var parameterInfos = t.GetProperties().Select(pi => new DbParameterInfo(pi));
                //var parameterInfos = _DbParameterInfoCache.GetParameterInfos(t, () => t.GetProperties().Select(pi => new DbParameterInfo(pi)));
                foreach (var pi in parameterInfos) {
                    AddParameter(cmd, pi.ParameterName, pi.GetValue(parameters));
                }
            }
        }

        public static void AppendOutParameters(this DbCommand cmd, IDictionary<string, int> outParameters) {
            if (outParameters == null)
                return;

            foreach (var kvp in outParameters) {
                var p = cmd.CreateParameter();
                p.ParameterName = "@" + kvp.Key;
                p.Direction = ParameterDirection.Output;
                p.Size = kvp.Value;
                cmd.Parameters.Add(p);
            }
        }

        public static IEnumerable<KeyValuePair<string, object>> GetReturnParametersValue(this DbCommand cmd) {
            foreach (DbParameter p in cmd.Parameters) {
                if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue || p.Direction == ParameterDirection.InputOutput)
                    yield return new KeyValuePair<string, object>(p.ParameterName, p.Value);
            }
        }

        private static void AddParameter(DbCommand cmd, string name, object value) {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
