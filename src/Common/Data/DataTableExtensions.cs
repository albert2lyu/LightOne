using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

namespace Vancl.WMS.Pilot.Common.Data {
    public static class DataTableExtensions {
        public static DataRow ImportDataReader(this DataTable table, DbDataReader dr) {
            var row = table.NewRow();
            for (var i = 0; i < dr.FieldCount; i++) {
                var columnName = dr.GetName(i);
                row[columnName] = dr[i];
            }
            table.Rows.Add(row);
            return row;
        }
    }
}
