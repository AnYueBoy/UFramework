/*
 * @Author: l hy
 * @Date: 2022-01-13 16:43:23
 * @Description: 事件派发
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFramework
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<string, IList<EventHandler<EventParam>>> handlerDic;

        public EventDispatcher()
        {
            handlerDic = new Dictionary<string, IList<EventHandler<EventParam>>>();
        }

        public bool AddListener(string eventName, EventHandler<EventParam> handler)
        {
            if (string.IsNullOrEmpty(eventName) || handler == null)
            {
                return false;
            }

            if (!handlerDic.TryGetValue(eventName, out IList<EventHandler<EventParam>> handlerList))
            {
                handlerDic[eventName] = handlerList = new List<EventHandler<EventParam>>();
            }

            if (handlerList.Contains(handler))
            {
                Debug.LogWarning(
                    $"Repeat event handler been added. eventName: {eventName}, handler {handler.GetType().Name}");
                return false;
            }

            handlerList.Add(handler);
            return true;
        }

        public void Raise(string eventName, object sender, EventParam e = null)
        {
            if (!handlerDic.TryGetValue(eventName, out IList<EventHandler<EventParam>> handlers))
            {
                return;
            }

            e ??= new EventParam();

            foreach (EventHandler<EventParam> listener in handlers)
            {
                if (e.IsStopEvent)
                {
                    listener(sender, e);
                    break;
                }

                listener(sender, e);
            }
        }

        public IList<EventHandler<EventParam>> GetListeners(string eventName)
        {
            if (handlerDic.ContainsKey(eventName))
            {
                return handlerDic[eventName];
            }

            return new List<EventHandler<EventParam>>();
        }

        public bool HasListener(string eventName)
        {
            return handlerDic.ContainsKey(eventName);
        }

        public bool RemoveListener(string eventName, EventHandler<EventParam> handler = null)
        {
            if (handler == null)
            {
                return handlerDic.Remove(eventName);
            }

            if (!handlerDic.TryGetValue(eventName, out IList<EventHandler<EventParam>> handlers))
            {
                return false;
            }

            bool status = handlers.Remove(handler);
            if (handlers.Count <= 0)
            {
                handlerDic.Remove(eventName);
            }

            return status;
        }
    }
}