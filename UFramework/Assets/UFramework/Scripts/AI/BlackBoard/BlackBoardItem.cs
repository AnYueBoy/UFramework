/*
 * @Author: l hy 
 * @Date: 2021-01-16 14:04:56 
 * @Description: 数据收集单元
 */
using UnityEngine;
namespace UFramework.AI.BehaviourTree {
    public class BlackBoardItem {

        private float m_expiredTime;
        private object m_value;
        public void SetValue (object target, float expiredTime = -1f) {
            m_value = target;
            m_expiredTime = expiredTime;

            if (expiredTime >= 0) {
                m_expiredTime = Time.time + expiredTime;
            }
        }

        public T GetValue<T> (T defaultValue) {
            if (IsValueValid ()) {
                return (T) m_value;
            }

            return defaultValue;
        }

        public bool IsValueValid () {
            return m_expiredTime < 0 || Time.time < m_expiredTime;
        }
    }
}