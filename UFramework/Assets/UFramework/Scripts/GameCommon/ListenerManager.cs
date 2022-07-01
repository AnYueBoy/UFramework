/*
 * @Author: l hy 
 * @Date: 2020-03-05 08:57:49 
 * @Description: 事件监听 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:43:03
 */

namespace UFramework.GameCommon
{
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class ListenerManager
    {
        private Dictionary<string, Dictionary<object, Delegate>> listenerMap =
            new Dictionary<string, Dictionary<object, Delegate>>();

        private Dictionary<object, List<string>> reverseListenerMap = new Dictionary<object, List<string>>();

        public void Add(string eventName, object caller, CallBack listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }
            else
            {
                listenerDic = new Dictionary<object, Delegate>();
                listenerMap.Add(eventName, listenerDic);
            }

            // 反向映射
            List<string> listenerNameList;
            if (reverseListenerMap.ContainsKey(caller))
            {
                listenerNameList = reverseListenerMap[caller];
            }
            else
            {
                listenerNameList = new List<string>();
                reverseListenerMap.Add(caller, listenerNameList);
            }

            CallBack listenerCall;
            if (listenerDic.ContainsKey(caller))
            {
                listenerCall = (CallBack) listenerDic[caller];
                listenerCall += listener;
            }
            else
            {
                listenerDic.Add(caller, listener);
            }

            // 反向映射
            if (listenerNameList.IndexOf(eventName) == -1)
            {
                listenerNameList.Add(eventName);
            }
        }

        public void Add<T>(string eventName, object caller, CallBack<T> listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }
            else
            {
                listenerDic = new Dictionary<object, Delegate>();
                listenerMap.Add(eventName, listenerDic);
            }

            // 反向映射
            List<string> listenerNameList;
            if (reverseListenerMap.ContainsKey(caller))
            {
                listenerNameList = reverseListenerMap[caller];
            }
            else
            {
                listenerNameList = new List<string>();
                reverseListenerMap.Add(caller, listenerNameList);
            }

            CallBack<T> listenerCall;
            if (listenerDic.ContainsKey(caller))
            {
                listenerCall = (CallBack<T>) listenerDic[caller];
                listenerCall += listener;
            }
            else
            {
                listenerDic.Add(caller, listener);
            }

            // 反向映射
            if (listenerNameList.IndexOf(eventName) == -1)
            {
                listenerNameList.Add(eventName);
            }
        }

        public void Add<T, X>(string eventName, object caller, CallBack<T, X> listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }
            else
            {
                listenerDic = new Dictionary<object, Delegate>();
                listenerMap.Add(eventName, listenerDic);
            }

            // 反向映射
            List<string> listenerNameList;
            if (reverseListenerMap.ContainsKey(caller))
            {
                listenerNameList = reverseListenerMap[caller];
            }
            else
            {
                listenerNameList = new List<string>();
                reverseListenerMap.Add(caller, listenerNameList);
            }

            CallBack<T, X> listenerCall;
            if (listenerDic.ContainsKey(caller))
            {
                listenerCall = (CallBack<T, X>) listenerDic[caller];
                listenerCall += listener;
            }
            else
            {
                listenerDic.Add(caller, listener);
            }

            // 反向映射
            if (listenerNameList.IndexOf(eventName) == -1)
            {
                listenerNameList.Add(eventName);
            }
        }

        public void Add<T, X, V>(string eventName, object caller, CallBack<T, X, V> listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }
            else
            {
                listenerDic = new Dictionary<object, Delegate>();
                listenerMap.Add(eventName, listenerDic);
            }

            // 反向映射
            List<string> listenerNameList;
            if (reverseListenerMap.ContainsKey(caller))
            {
                listenerNameList = reverseListenerMap[caller];
            }
            else
            {
                listenerNameList = new List<string>();
                reverseListenerMap.Add(caller, listenerNameList);
            }

            CallBack<T, X, V> listenerCall;
            if (listenerDic.ContainsKey(caller))
            {
                listenerCall = (CallBack<T, X, V>) listenerDic[caller];
                listenerCall += listener;
            }
            else
            {
                listenerDic.Add(caller, listener);
            }

            // 反向映射
            if (listenerNameList.IndexOf(eventName) == -1)
            {
                listenerNameList.Add(eventName);
            }
        }

        public void Add<T, X, V, M>(string eventName, object caller, CallBack<T, X, V, M> listener)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            if (listener == null)
            {
                Debug.LogError("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }
            else
            {
                listenerDic = new Dictionary<object, Delegate>();
                listenerMap.Add(eventName, listenerDic);
            }

            // 反向映射
            List<string> listenerNameList;
            if (reverseListenerMap.ContainsKey(caller))
            {
                listenerNameList = reverseListenerMap[caller];
            }
            else
            {
                listenerNameList = new List<string>();
                reverseListenerMap.Add(caller, listenerNameList);
            }

            CallBack<T, X, V, M> listenerCall;
            if (listenerDic.ContainsKey(caller))
            {
                listenerCall = (CallBack<T, X, V, M>) listenerDic[caller];
                listenerCall += listener;
            }
            else
            {
                listenerDic.Add(caller, listener);
            }

            // 反向映射
            if (listenerNameList.IndexOf(eventName) == -1)
            {
                listenerNameList.Add(eventName);
            }
        }

        public void RemoveAll(object caller)
        {
            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            List<string> eventNameList = reverseListenerMap[caller];
            if (eventNameList == null)
            {
                Debug.LogError("target event list not exist");
                return;
            }

            foreach (string eventName in eventNameList)
            {
                if (!listenerMap.ContainsKey(eventName))
                {
                    continue;
                }

                Dictionary<object, Delegate> listenerDic = listenerMap[eventName];
                if (!listenerDic.ContainsKey(caller))
                {
                    continue;
                }

                listenerDic.Remove(caller);
            }

            reverseListenerMap.Remove(caller);
        }

        public void RemoveAt(string eventName, object caller)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("Listener eventName is null!");
                return;
            }

            if (caller == null)
            {
                Debug.LogError("caller is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("remove fail not exist event: " + eventName);
                return;
            }

            if (!listenerDic.ContainsKey(caller))
            {
                Debug.LogError("remove fail not exist caller: " + caller);
                return;
            }

            listenerDic.Remove(caller);

            List<string> eventNameList = reverseListenerMap[caller];
            eventNameList.Remove(eventName);
        }

        public void Trigger(string eventName)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack call in listenerDic.Values)
            {
                call();
            }
        }

        public void Trigger<T>(string eventName, T arg)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T> call in listenerDic.Values)
            {
                call(arg);
            }
        }

        public void Trigger<T, X>(string eventName, T arg1, X arg2)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X> call in listenerDic.Values)
            {
                call(arg1, arg2);
            }
        }

        public void Trigger<T, X, V>(string eventName, T arg1, X arg2, V arg3)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X, V> call in listenerDic.Values)
            {
                call(arg1, arg2, arg3);
            }
        }

        public void Trigger<T, X, V, M>(string eventName, T arg1, X arg2, V arg3, M arg4)
        {
            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey(eventName))
            {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null)
            {
                Debug.LogWarning("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X, V, M> call in listenerDic.Values)
            {
                call(arg1, arg2, arg3, arg4);
            }
        }
    }

    public delegate void CallBack();

    public delegate void CallBack<in T>(T arg);

    public delegate void CallBack<in T, in X>(T arg1, X arg2);

    public delegate void CallBack<in T, in X, in V>(T arg1, X arg2, V arg3);

    public delegate void CallBack<in T, in X, in V, in M>(T arg1, X arg2, V arg3, M arg4);
}