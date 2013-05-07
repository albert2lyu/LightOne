using System;
using System.Linq;
using System.Data.Common;

namespace Common.Data {
    public static class DbDataReaderExtensions {
        public static bool HasColumn(this DbDataReader dr, string columnName) {
            for (var i = 0; i < dr.FieldCount; i++) {
                if (string.Compare(dr.GetName(i), columnName, true) == 0)
                    return true;
            }
            return false;
        }
    }
}
