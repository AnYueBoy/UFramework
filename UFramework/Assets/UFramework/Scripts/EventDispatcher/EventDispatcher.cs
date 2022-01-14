/*
 * @Author: l hy 
 * @Date: 2022-01-13 16:43:23 
 * @Description: 事件派发
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UFramework.Exception;
using UFramework.Util;

namespace UFramework.EventDispatcher {
    public class EventDispatcher : IEventDispatcher {

        private readonly Dictionary<string, IList<EventHandler>> listeners;

        public EventDispatcher () {
            listeners = new Dictionary<string, IList<EventHandler>> ();
        }

        public virtual bool AddListener (string eventName, EventHandler handler) {
            if (string.IsNullOrEmpty (eventName) || handler == null) {
                return false;
            }

            if (!listeners.TryGetValue (eventName, out IList<EventHandler> handlers)) {
                listeners[eventName] = handlers = new List<EventHandler> ();
            } else if (handlers.Contains (handler)) {
                return false;
            }

            handlers.Add (handler);
            return true;
        }

        public void Raise (string eventName, object sender, EventArgs e = null) {
            Guard.Requires<LogicException> (!(sender is EventArgs), $"Passed event args for the parameter {sender},Did you make a wrong method call?");

            e = e?? EventArgs.Empty;
            if (!listeners.TryGetValue (eventName, out IList<EventHandler> handlers)) {
                return;
            }

            foreach (var listener in handlers) {
                if (e is IStoppableEvent stoppableEvent && stoppableEvent.IsPropagationStopped) {
                    break;
                }

                listener (sender, e);
            }

        }

        public virtual EventHandler[] GetListeners (string eventName) {
            if (!listeners.TryGetValue (eventName, out IList<EventHandler> handlers)) {
                return Array.Empty<EventHandler> ();
            }

            return handlers.ToArray ();
        }

        public bool HasListener (string eventName) {
            return listeners.ContainsKey (eventName);
        }

        public bool RemoveListener (string eventName, EventHandler handler = null) {
            if (handler == null) {
                return listeners.Remove (eventName);
            }

            if (!listeners.TryGetValue (eventName, out IList<EventHandler> handlers)) {
                return false;
            }

            bool status = handlers.Remove (handler);
            if (handlers.Count <= 0) {
                listeners.Remove (eventName);
            }

            return status;
        }
    }
}