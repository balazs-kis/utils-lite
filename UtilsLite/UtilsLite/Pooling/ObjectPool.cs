using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UtilsLite.Events;

namespace UtilsLite.Pooling
{
    /// <summary>
    /// A simple generic pool implementation, with a Get and Release method.
    /// The maximum size of the pool can be dynamically modified. 
    /// </summary>
    /// <typeparam name="T">The type of the objects the pool holds</typeparam>
    public class ObjectPool<T> : IObjectPool<T> where T : class
    {
        private readonly ConcurrentBag<T> _available;
        private readonly ConcurrentDictionary<int, DateTime> _creationTimes;
        private readonly Func<T> _createObject;
        private readonly bool _discardUsed;
        private readonly TimeSpan? _maxAge;
        private readonly Action<T> _releaseAction;

        public CustomEvent<T> OnAcquiring { get; }

        private int _inUseNumber;
        private int _maxSize;

        public int AvailableTokenCount => _maxSize - _inUseNumber;


        public ObjectPool(
            Func<T> createObject,
            int maxSize,
            int initialSize = 0,
            bool discardUsed = false,
            TimeSpan? maxAge = null,
            Action<T> releaseAction = null)
        {
            OnAcquiring = new CustomEvent<T>();
            _creationTimes = new ConcurrentDictionary<int, DateTime>();

            _createObject = createObject;
            _discardUsed = discardUsed;
            _maxAge = maxAge;
            _releaseAction = releaseAction;

            _inUseNumber = 0;
            _maxSize = maxSize;

            initialSize = Math.Min(initialSize, maxSize);

            var initialObjects = new T[initialSize];
            for (var i = 0; i < initialSize; i++)
            {
                initialObjects[i] = CreateObject();
            }

            _available = new ConcurrentBag<T>(initialObjects);
        }


        /// <summary>
        /// Returns an available object from the pool.
        /// If there is no available object, returns null.
        /// </summary>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        public T AcquireObject()
        {
            var item = default(T);
            var taken = false;
            var isOk = false;

            while (!isOk)
            {
                taken = _available.TryTake(out item);
                var isOld = IsObjectOld(item);
                isOk = !taken || !isOld;

                if (taken && isOld)
                {
                    ReleasePoolItem(item);
                    item = default;
                }

                if (taken && !isOld)
                {
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        OnAcquiring?.Invoke(item);
                        sw.Stop();
                    }
                    catch
                    {
                        isOk = false;
                    }
                }
            }

            if (taken && item != default)
            {
                Interlocked.Increment(ref _inUseNumber);
                return item;
            }

            var creationRetries = 0;

            while (item == default && creationRetries < 5)
            {

                if (_available.Count + _inUseNumber >= _maxSize)
                {
                    throw new NoAvailableItemsException();
                }

                try
                {
                    var ok = _available.TryTake(out item);
                    if (!ok)
                    {
                        item = CreateObject();
                    }

                    OnAcquiring?.Invoke(item);
                    Interlocked.Increment(ref _inUseNumber);
                }
                catch
                {
                    item = default;
                }

                creationRetries++;
            }

            if (item == default)
            {
                throw new NoAvailableItemsException();
            }

            return item;
        }

        /// <summary>
        /// Returns a collection of available object from the pool.
        /// The collections size is equal or less than the given count.
        /// If there is no available object, returns an empty collection.
        /// </summary>
        /// <param name="count">The number of objects to acquire</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        public IEnumerable<T> AcquireObjects(int count)
        {
            var result = new List<T>();

            for (var i = 0; i < Math.Min(count, _maxSize); i++)
            {
                var o = AcquireObject();
                if (o != null) result.Add(o);
            }

            return result;
        }

        /// <summary>
        /// Returns a collection of all available object from the pool.
        /// If there is no available object, returns an empty collection.
        /// </summary>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        public IEnumerable<T> AcquireAllObjects()
        {
            return AcquireObjects(_maxSize);
        }

        /// <summary>
        /// Releases an object  back to the pool.
        /// </summary>
        /// <param name="item">The object to release</param>
        /// <param name="discard">If true, the item will be discarded instead of returning it to the pool</param>
        public virtual void ReleaseObject(T item, bool discard = false)
        {
            if (_inUseNumber <= 0)
            {
                throw new InvalidOperationException("Trying to release an object, but no object is in use");
            }

            var isPoolOversized = _available.Count + _inUseNumber > _maxSize;

            if (!_discardUsed && !discard && !IsObjectOld(item) && !isPoolOversized)
            {
                _available.Add(item);
            }
            else
            {
                ReleasePoolItem(item);
            }

            Interlocked.Decrement(ref _inUseNumber);
        }

        /// <summary>
        /// Acquires an object from the pool, executes the given action with that object,
        /// then returns the object to the pool. If an object is not available from the pool,
        /// throws an exception.
        /// </summary>
        /// <param name="action">The action to execute with the acquired object</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        public void ExecuteWithObject(Action<T> action)
        {
            ExecuteWithObject(i =>
            {
                action.Invoke(i);
                return default(T);
            });
        }

        /// <summary>
        /// Acquires an object from the pool, executes the given function with that object,
        /// then returns the object to the pool. If an object is not available from the pool,
        /// throws an exception.
        /// </summary>
        /// <param name="func">The function to call with the acquired object</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        public TResult ExecuteWithObject<TResult>(Func<T, TResult> func)
        {
            var acquiredObject = AcquireObject();

            if (acquiredObject == null)
            {
                throw new Exception("Could not acquire object");
            }

            try
            {
                var res = func.Invoke(acquiredObject);
                ReleaseObject(acquiredObject);
                return res;
            }
            catch
            {
                ReleaseObject(acquiredObject, true);
                throw;
            }
        }

        /// <summary>
        /// Modifies the pool size to the given value.
        /// </summary>
        /// <param name="newSize">The new size of the pool</param>
        public void ModifyPoolSize(int newSize)
        {
            if (_maxSize != newSize) Interlocked.Exchange(ref _maxSize, newSize);
        }


        private T CreateObject()
        {
            var o = _createObject();
            _creationTimes.AddOrUpdate(o.GetHashCode(), DateTime.UtcNow, (hash, createdAt) => DateTime.UtcNow);
            return o;
        }

        private bool IsObjectOld(T item)
        {
            if (!_maxAge.HasValue || item == default(T))
            {
                return false;
            }

            var hash = item.GetHashCode();
            if (!_creationTimes.ContainsKey(hash))
            {
                throw new Exception("The item's hash was not found in the pool's dictionary");
            }

            return DateTime.UtcNow - _creationTimes[hash] > _maxAge.Value;
        }

        private void ReleasePoolItem(T item)
        {
            Task.Run(() =>
            {
                var hash = item.GetHashCode();

                try
                {
                    _releaseAction?.Invoke(item);
                }
                finally
                {
                    _creationTimes.TryRemove(hash, out _);
                }
            });
        }
    }
}