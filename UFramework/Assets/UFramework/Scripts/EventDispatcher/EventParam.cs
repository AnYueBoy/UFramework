/*
 * @Author: l hy 
 * @Date: 2022-01-14 14:04:03 
 * @Description: 通用事件参数
 */

using System;
namespace UFramework.EventDispatcher {
    public class EventParam {
        private object[] _data;

        public EventParam (params object[] data) {
            _data = data;
        }

        public object[] value {
            get {
                return _data;
            }
        }
    }
}