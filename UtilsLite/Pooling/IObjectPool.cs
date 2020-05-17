using System;
using System.Collections.Generic;
using UtilsLite.Events;

namespace UtilsLite.Pooling
{
    /// <summary>
    /// A simple generic pool interface, with a Get and Release method,
    /// and a method to dynamically change the maximum size of the pool. 
    /// </summary>
    /// <typeparam name="T">The type of the objects the pool holds</typeparam>
    public interface IObjectPool<T> where T : class
    {
        /// <summary>
        /// Occurs after an object is acquired and before it is handed out.
        /// </summary>
        CustomEvent<T> OnAcquiring { get; }

        int AvailableTokenCount { get; }

        /// <summary>
        /// Returns an available object from the pool.
        /// If there is no available object, returns null.
        /// </summary>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        T AcquireObject();

        /// <summary>
        /// Returns a collection of available object from the pool.
        /// The collections size is equal or less than the given count.
        /// If there is no available object, returns an empty collection.
        /// </summary>
        /// <param name="count">The number of objects to acquire</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        IEnumerable<T> AcquireObjects(int count);

        /// <summary>
        /// Returns a collection of all available object from the pool.
        /// If there is no available object, returns an empty collection.
        /// </summary>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        IEnumerable<T> AcquireAllObjects();

        /// <summary>
        /// Releases an object  back to the pool.
        /// </summary>
        /// <param name="item">The object to release</param>
        /// <param name="discard">If true, the item will be discarded instead of returning it to the pool</param>
        void ReleaseObject(T item, bool discard = false);

        /// <summary>
        /// Acquires an object from the pool, executes the given action  with that object,
        /// then returns the object to the pool. If an object is not available from the pool,
        /// this method will wait exponentially increasing periods of time, then,
        /// if still not successful, will throw an exception.
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        void ExecuteWithObject(Action<T> action);

        /// <summary>
        /// Acquires an object from the pool, executes the given action  with that object,
        /// then returns the object to the pool. If an object is not available from the pool,
        /// this method will wait exponentially increasing periods of time, then,
        /// if still not successful, will throw an exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="func">The function to call</param>
        /// <exception cref="NoAvailableItemsException">When no items are available</exception>
        TResult ExecuteWithObject<TResult>(Func<T, TResult> func);

        /// <summary>
        /// Modifies the pool size to the given value.
        /// </summary>
        /// <param name="newSize">The new size of the pool</param>
        void ModifyPoolSize(int newSize);

    }
}