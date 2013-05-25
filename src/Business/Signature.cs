using System;
using System.Linq;

namespace Business
{
    public class Signature {
        public byte[] Data { get; set; }

        public Signature(byte[] data) {
            if (data == null || data.Length == 0)
                throw new ArgumentException("签名数据不能为空");

            Data = data;
        }

        public static bool IsMatch(Signature sig1, Signature sig2) {
            if (sig1 == null || sig1.Data.Length == 0 ||
                sig2 == null || sig2.Data.Length == 0 ||
                sig1.Data.Length != sig2.Data.Length)
                return false;

            for (var i = 0; i < sig1.Data.Length; i++) {
                if (sig1.Data[i] != sig2.Data[i])
                    return false;
            }

            return true;
        }
    }
}
