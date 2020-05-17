using System;
using System.Collections.Generic;

namespace UtilsLite.Events
{
    /// <summary>
    /// Unlike regular events, one specific action can be subscribed only once;
    /// further registrations of the same action will be ignored.
    /// </summary>
    /// <typeparam name="T">Type of the event object</typeparam>
    public class CustomEvent<T>
    {
        private readonly List<Action<T>> _actions = new List<Action<T>>();

        public void Invoke(T p)
        {
            _actions.ForEach(a => a.Invoke(p));
        }

        /// <summary>
        /// Subscribes the specified action.
        /// Unlike regular events, one specific action can be subscribed only once;
        /// further registrations of the same action will be ignored.
        /// </summary>
        /// <param name="action">The action to subscribe</param>
        /// <returns>Returns true, if the action was subscribed and false if it was already registered.</returns>
        public bool Subscribe(Action<T> action)
        {
            if (!_actions.Contains(action))
            {
                _actions.Add(action);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unsubscribes the specified action.
        /// </summary>
        /// <param name="action">The action to unsubscribe</param>
        /// <returns>Returns true, if the action was unsubscribed and false if it was not registered.</returns>
        public bool Unsubscribe(Action<T> action)
        {
            if (_actions.Contains(action))
            {
                _actions.Remove(action);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Subscribes the specified action.
        /// Unlike regular events, one specific action can be subscribed only once;
        /// further registrations of the same action will be ignored.
        /// </summary>
        /// <param name="e">The event to subscribe to</param>
        /// <param name="action">The action to subscribe</param>
        public static CustomEvent<T> operator +(CustomEvent<T> e, Action<T> action)
        {
            e.Subscribe(action);
            return e;
        }

        /// <summary>
        /// Unsubscribes the specified action.
        /// </summary>
        /// <param name="e">The event to unsubscribe from</param>
        /// <param name="action">The action to unsubscribe</param>
        public static CustomEvent<T> operator -(CustomEvent<T> e, Action<T> action)
        {
            e.Unsubscribe(action);
            return e;
        }
    }
}