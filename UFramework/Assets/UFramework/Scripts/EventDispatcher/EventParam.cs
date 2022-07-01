/*
 * @Author: l hy 
 * @Date: 2022-01-14 14:04:03 
 * @Description: 通用事件参数
 */

using System;
namespace UFramework.EventDispatcher {
    public class EventParam {
        private readonly object[] data;

        public EventParam (params object[] data) {
            this.data = data;
        }

        public object[] Value => data;
    }
}