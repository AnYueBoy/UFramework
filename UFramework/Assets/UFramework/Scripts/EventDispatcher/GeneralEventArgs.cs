/*
 * @Author: l hy 
 * @Date: 2022-01-14 14:04:03 
 * @Description: 通用事件参数
 */

using System;
namespace UFramework.EventDispatcher {
    public class GeneralEventArgs : EventArgs {
        private object _data;

        public GeneralEventArgs (object data = null) {
            _data = data;
        }

        public object Data {
            get {
                return _data;
            }
        }
    }
}