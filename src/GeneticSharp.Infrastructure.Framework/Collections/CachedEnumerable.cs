using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticSharp.Infrastructure.Framework.Collections
{

    public sealed class CachedEnumerable<T> : IEnumerable<T>, IDisposable
    {
        private readonly List<T> _cache = new List<T>();
        private readonly IEnumerable<T> _enumerable;
        private IEnumerator<T> _enumerator;
        private bool _enumerated = false;

        public CachedEnumerable(IEnumerable<T> enumerable)
        {
            _enumerable = enumerable ?? throw new ArgumentNullException(nameof(enumerable));
        }

        public IEnumerator<T> GetEnumerator()
        {
            var index = 0;
            while (true)
            {
                if (TryGetItem(index, out var result))
                {
                    yield return result;
                    index++;
                }
                else
                {
                    // There are no more items
                    yield break;
                }
            }
        }

        private bool TryGetItem(int index, out T result)
        {
            // if the item is in the cache, use it
            if (index < _cache.Count)
            {
                result = _cache[index];
                return true;
            }

            lock (_cache)
            {
                if (_enumerator == null && !_enumerated)
                {
                    _enumerator = _enumerable.GetEnumerator();
                }

                // Another thread may have get the item while we were acquiring the lock
                if (index < _cache.Count)
                {
                    result = _cache[index];
                    return true;
                }

                // If we have already enumerate the whole stream, there is nothing else to do
                if (_enumerated)
                {
                    result = default;
                    return false;
                }

                // Get the next item and store it to the cache
                if (_enumerator.MoveNext())
                {
                    result = _enumerator.Current;
                    _cache.Add(result);
                    return true;
                }
                else
                {
                    // There are no more items, we can dispose the underlying enumerator
                    _enumerator.Dispose();
                    _enumerator = null;
                    _enumerated = true;
                    result = default;
                    return false;
                }
            }
        }

        public void Dispose()
        {
            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


}