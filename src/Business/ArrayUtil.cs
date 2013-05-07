using System;
using System.Linq;

namespace Business
{
    public static class ArrayUtil {
        public static bool Equals(string[] array1, string[] array2) {
            if (array1 == null && array2 == null)
                return true;

            if (array1 != null && array2 != null &&
                array1.Length == array2.Length) {
                for (var i = 0; i < array1.Length; i++) {
                    if (array1[i] != array2[i])
                        return false;
                }
                return true;
            }

            return false;
        }
    }
}
