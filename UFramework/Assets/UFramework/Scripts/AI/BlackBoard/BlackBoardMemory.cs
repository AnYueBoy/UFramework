/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:03:16 
 * @Description:数据收集
 */

using System.Collections.Generic;
namespace UFramework.AI.BehaviourTree {
    public class BlackBoardMemory {
        private Dictionary<int, BlackBoardItem> m_items;

        public BlackBoardMemory () {
            this.m_items = new Dictionary<int, BlackBoardItem> ();
        }

        public void clear () {
            this.m_items.Clear ();
        }

        public void setValue (int key, object target, float expiredTime = -1f) {
            BlackBoardItem item;
            if (!m_items.ContainsKey (key)) {
                item = new BlackBoardItem ();
                m_items.Add (key, item);
            } else {
                item = m_items[key];
            }

            item.setValue (target, expiredTime);
        }

        public bool hasValue (int key) {
            BlackBoardItem item;
            if (!m_items.TryGetValue (key, out item)) {
                return false;
            }

            return item.isValueValid ();
        }

        public void delValue (int key) {
            m_items.Remove (key);
        }

        public T getValue<T> (int key, T defaultValue = default (T)) {
            BlackBoardItem item;
            if (!m_items.TryGetValue (key, out item)) {
                return defaultValue;
            }

            return item.getValue<T> (defaultValue);
        }

        public bool tryGetValue<T> (int key, out T value) {
            BlackBoardItem item;
            if (!m_items.TryGetValue (key, out item) || !item.isValueValid ()) {
                value = default (T);
                return false;
            }

            value = item.getValue<T> (default (T));
            return true;
        }
    }
}