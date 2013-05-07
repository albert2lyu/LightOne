using System;
using System.Linq;

namespace Business
{
    class Signature {
        private readonly byte[] _Data;

        public Signature(byte[] data) {
            if (data == null || data.Length == 0)
                throw new ArgumentException("签名数据不能为空");

            _Data = data;
        }

        public static bool IsMatch(Signature sig1, Signature sig2) {
            if (sig1 == null || sig1._Data.Length == 0 ||
                sig2 == null || sig2._Data.Length == 0 ||
                sig1._Data.Length != sig2._Data.Length)
                return false;

            for (var i = 0; i < sig1._Data.Length; i++) {
                if (sig1._Data[i] != sig2._Data[i])
                    return false;
            }

            return true;
        }
    }
}
