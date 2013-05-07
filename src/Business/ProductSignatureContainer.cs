using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Business {
    class ProductSignatureContainer {
        private struct ProductSignatureContainerKey {
            public string Source { get; set; }

            public string Number { get; set; }

            public ProductSignatureContainerKey(string source, string number) :
                this() {
                if (source == null)
                    throw new ArgumentNullException("source");

                if (number == null)
                    throw new ArgumentNullException("number");

                Source = source;
                Number = number;
            }

            public override int GetHashCode() {
                return 13 * Source.GetHashCode() +
                    17 * Number.GetHashCode();
            }

            public override bool Equals(object obj) {
                if (obj == null)
                    return false;
                if (!(obj is ProductSignatureContainerKey))
                    return false;

                var key = (ProductSignatureContainerKey)obj;
                return key.Source == Source && key.Number == Number;
            }
        }

        private readonly static ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private readonly static IDictionary<ProductSignatureContainerKey, Signature> ProductSignatures = new Dictionary<ProductSignatureContainerKey, Signature>();

        public void Add(string source, string number, Signature signature) {
            Lock.EnterWriteLock();
            try {
                if (signature == null)
                    throw new ArgumentException("签名不能为空");

                var key = new ProductSignatureContainerKey(source, number);
                if (ProductSignatures.ContainsKey(key))
                    ProductSignatures[key] = signature;
                else
                    ProductSignatures.Add(key, signature);
            }
            finally {
                Lock.ExitWriteLock();
            }
        }

        public bool Contains(string source, string number, Signature signature) {
            Lock.EnterReadLock();
            try {
                var key = new ProductSignatureContainerKey(source, number);
                if (!ProductSignatures.ContainsKey(key))
                    return false;

                return Signature.IsMatch(signature, ProductSignatures[key]);
            }
            finally {
                Lock.ExitReadLock();
            }
        }
    }
}
