﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common {
    public abstract class ReaderWriterCache<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        protected ReaderWriterCache()
            : this(null) {
        }

        protected ReaderWriterCache(IEqualityComparer<TKey> comparer) {
            _cache = new Dictionary<TKey, TValue>(comparer);
        }

        protected Dictionary<TKey, TValue> Cache {
            get { return _cache; }
        }

        protected TValue FetchOrCreateItem(TKey key, Func<TValue> creator) {
            // first, see if the item already exists in the cache
            _readerWriterLock.EnterReadLock();
            try {
                TValue existingEntry;
                if (_cache.TryGetValue(key, out existingEntry)) {
                    return existingEntry;
                }
            }
            finally {
                _readerWriterLock.ExitReadLock();
            }

            // insert the new item into the cache
            TValue newEntry = creator();
            _readerWriterLock.EnterWriteLock();
            try {
                TValue existingEntry;
                if (_cache.TryGetValue(key, out existingEntry)) {
                    // another thread already inserted an item, so use that one
                    return existingEntry;
                }

                _cache[key] = newEntry;
                return newEntry;
            }
            finally {
                _readerWriterLock.ExitWriteLock();
            }
        }
    }
}
