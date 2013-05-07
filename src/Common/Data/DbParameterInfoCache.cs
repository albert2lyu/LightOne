using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Data {
    class DbParameterInfoCache : ReaderWriterCache<Type, IEnumerable<DbParameterInfo>> {
        public DbParameterInfoCache() {
        }

        public IEnumerable<DbParameterInfo> GetParameterInfos(Type type, Func<IEnumerable<DbParameterInfo>> creator) {
            return FetchOrCreateItem(type, creator);
        }
    }
}
