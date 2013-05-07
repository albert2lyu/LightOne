using System;
using System.Linq;

namespace Common.Data
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class AutoTruncateAttribute : Attribute {
        public int MaxLength { get; set; }

        public AutoTruncateAttribute(int maxLength) {
            MaxLength = maxLength;
        }
    }
}
