using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Vancl.WMS.Pilot.Common.Data {
    public static class DataViewExtensions {
        public static IEnumerable<T> Select<T>(this DataView view, Func<DataRowView, T> action) {
            foreach (DataRowView row in view) {
                if (action != null)
                    yield return action.Invoke(row);
            }
        }
    }
}
