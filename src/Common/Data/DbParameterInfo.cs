using System;
using System.Linq;
using System.Reflection;

namespace Common.Data
{
    class DbParameterInfo {
        private readonly PropertyInfo _PropertyInfo;

        private readonly int? _MaxLength;

        public string ParameterName { get { return _PropertyInfo.Name; } }

        public DbParameterInfo(PropertyInfo pi) {
            if (pi == null)
                throw new ArgumentNullException("pi");

            _PropertyInfo = pi;
            _MaxLength = GetMaxLength(pi);
        }

        public object GetValue(object obj) {
            var paramValue = _PropertyInfo.GetValue(obj, null);
            var propertyType = _PropertyInfo.PropertyType;
            if (propertyType == typeof(string)) {
                paramValue = TruncateByMaxLength((string)paramValue);
            }
            else if (propertyType.IsArray && propertyType.GetElementType() == typeof(string)) {
                // 字符串数组
                var stringArray = paramValue as string[];
                for (var i = 0; i < stringArray.Length; i++) {
                    stringArray[i] = TruncateByMaxLength(stringArray[i]);
                }
            }

            return paramValue;
        }

        private string TruncateByMaxLength(string str) {
            if (_MaxLength.HasValue && str != null && str.Length > _MaxLength.Value)
                return str.Substring(0, _MaxLength.Value);
            return str;
        }

        private int? GetMaxLength(PropertyInfo pi) {
            var attrs = (AutoTruncateAttribute[])pi.GetCustomAttributes(typeof(AutoTruncateAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return attrs[0].MaxLength;
            return null;
        }
    }
}
