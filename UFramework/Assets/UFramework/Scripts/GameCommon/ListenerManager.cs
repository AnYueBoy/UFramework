/*
 * @Author: l hy 
 * @Date: 2020-03-05 08:57:49 
 * @Description: 事件监听 
 * @Last Modified by: l hy
 * @Last Modified time: 2020-12-21 16:43:03
 */
namespace UFramework.GameCommon {

    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class ListenerManager {

        private static ListenerManager instance = null;

        public static ListenerManager getInstance () {
            if (instance == null) {
                instance = new ListenerManager ();
            }
            return instance;
        }
        private Dictionary<string, Dictionary<object, Delegate>> listenerMap = new Dictionary<string, Dictionary<object, Delegate>> ();

        public void add (string eventName, object caller, CallBack listener) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            if (listener == null) {
                Debug.LogError ("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            } else {
                listenerDic = new Dictionary<object, Delegate> ();
                listenerMap.Add (eventName, listenerDic);
            }

            CallBack listenerCall;
            if (listenerDic.ContainsKey (caller)) {
                listenerCall = (CallBack) listenerDic[caller];
                listenerCall += listener;
            } else {
                listenerDic.Add (caller, listener);
            }
        }

        public void add<T> (string eventName, object caller, CallBack<T> listener) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            if (listener == null) {
                Debug.LogError ("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            } else {
                listenerDic = new Dictionary<object, Delegate> ();
                listenerMap.Add (eventName, listenerDic);
            }

            CallBack<T> listenerCall;
            if (listenerDic.ContainsKey (caller)) {
                listenerCall = (CallBack<T>) listenerDic[caller];
                listenerCall += listener;
            } else {
                listenerDic.Add (caller, listener);
            }
        }

        public void add<T, X> (string eventName, object caller, CallBack<T, X> listener) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            if (listener == null) {
                Debug.LogError ("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            } else {
                listenerDic = new Dictionary<object, Delegate> ();
                listenerMap.Add (eventName, listenerDic);
            }

            CallBack<T, X> listenerCall;
            if (listenerDic.ContainsKey (caller)) {
                listenerCall = (CallBack<T, X>) listenerDic[caller];
                listenerCall += listener;
            } else {
                listenerDic.Add (caller, listener);
            }
        }

        public void add<T, X, V> (string eventName, object caller, CallBack<T, X, V> listener) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            if (listener == null) {
                Debug.LogError ("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            } else {
                listenerDic = new Dictionary<object, Delegate> ();
                listenerMap.Add (eventName, listenerDic);
            }

            CallBack<T, X, V> listenerCall;
            if (listenerDic.ContainsKey (caller)) {
                listenerCall = (CallBack<T, X, V>) listenerDic[caller];
                listenerCall += listener;
            } else {
                listenerDic.Add (caller, listener);
            }
        }

        public void add<T, X, V, M> (string eventName, object caller, CallBack<T, X, V, M> listener) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            if (listener == null) {
                Debug.LogError ("listener is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            } else {
                listenerDic = new Dictionary<object, Delegate> ();
                listenerMap.Add (eventName, listenerDic);
            }

            CallBack<T, X, V, M> listenerCall;
            if (listenerDic.ContainsKey (caller)) {
                listenerCall = (CallBack<T, X, V, M>) listenerDic[caller];
                listenerCall += listener;
            } else {
                listenerDic.Add (caller, listener);
            }
        }

        public void removeAll (string eventName, object caller) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("Listener eventName is null!");
                return;
            }

            if (caller == null) {
                Debug.LogError ("caller is null");
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (this.listenerMap.ContainsKey (eventName)) {
                listenerDic = this.listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("remove fail not exist event: " + eventName);
                return;
            }

            if (listenerDic.ContainsKey (caller)) {
                listenerDic.Remove (caller);
            } else {
                Debug.LogError ("remove fail not exist caller: " + caller);
            }
        }

        public void trigger (string eventName) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey (eventName)) {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack call in listenerDic.Values) {
                call ();
            }
        }

        public void trigger<T> (string eventName, T arg) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey (eventName)) {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T> call in listenerDic.Values) {
                call (arg);
            }
        }

        public void trigger<T, X> (string eventName, T arg1, X arg2) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey (eventName)) {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X> call in listenerDic.Values) {
                call (arg1, arg2);
            }
        }

        public void trigger<T, X, V> (string eventName, T arg1, X arg2, V arg3) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey (eventName)) {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X, V> call in listenerDic.Values) {
                call (arg1, arg2, arg3);
            }
        }

        public void trigger<T, X, V, M> (string eventName, T arg1, X arg2, V arg3, M arg4) {
            if (eventName == null || eventName == "") {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            Dictionary<object, Delegate> listenerDic = null;
            if (listenerMap.ContainsKey (eventName)) {
                listenerDic = listenerMap[eventName];
            }

            if (listenerDic == null) {
                Debug.LogError ("not exist eventName: " + eventName);
                return;
            }

            foreach (CallBack<T, X, V, M> call in listenerDic.Values) {
                call (arg1, arg2, arg3, arg4);
            }
        }

    }

    public delegate void CallBack ();

    public delegate void CallBack<T> (T arg);

    public delegate void CallBack<T, X> (T arg1, X arg2);

    public delegate void CallBack<T, X, V> (T arg1, X arg2, V arg3);

    public delegate void CallBack<T, X, V, M> (T arg1, X arg2, V arg3, M arg4);
}