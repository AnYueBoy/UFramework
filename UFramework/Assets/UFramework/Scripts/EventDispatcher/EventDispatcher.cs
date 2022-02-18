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

        private readonly Dictionary<string, IList<EventHandler<EventParam>>> listeners;

        public EventDispatcher () {
            listeners = new Dictionary<string, IList<EventHandler<EventParam>>> ();
        }

        public virtual bool AddListener (string eventName, EventHandler<EventParam> handler) {
            if (string.IsNullOrEmpty (eventName) || handler == null) {
                return false;
            }

            if (!listeners.TryGetValue (eventName, out IList<EventHandler<EventParam>> handlers)) {
                listeners[eventName] = handlers = new List<EventHandler<EventParam>> ();
            } else if (handlers.Contains (handler)) {
                return false;
            }

            handlers.Add (handler);
            return true;
        }

        public void Raise (string eventName, object sender, EventParam e = null) {
            Guard.Requires<LogicException> (!(sender is EventArgs), $"Passed event args for the parameter {sender},Did you make a wrong method call?");
            e = e??new EventParam ();
            if (!listeners.TryGetValue (eventName, out IList<EventHandler<EventParam>> handlers)) {
                return;
            }

            foreach (EventHandler<EventParam> listener in handlers) {
                if (e is IStoppableEvent stoppableEvent && stoppableEvent.IsPropagationStopped) {
                    break;
                }

                listener (sender, e);
            }

        }

        public virtual EventHandler<EventParam>[] GetListeners (string eventName) {
            if (!listeners.TryGetValue (eventName, out IList<EventHandler<EventParam>> handlers)) {
                return Array.Empty<EventHandler<EventParam>> ();
            }

            return handlers.ToArray ();
        }

        public bool HasListener (string eventName) {
            return listeners.ContainsKey (eventName);
        }

        public bool RemoveListener (string eventName, EventHandler<EventParam> handler = null) {
            if (handler == null) {
                return listeners.Remove (eventName);
            }

            if (!listeners.TryGetValue (eventName, out IList<EventHandler<EventParam>> handlers)) {
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