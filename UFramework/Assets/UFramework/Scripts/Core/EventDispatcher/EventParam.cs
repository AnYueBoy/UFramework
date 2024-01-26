/*
 * @Author: l hy
 * @Date: 2022-01-14 14:04:03
 * @Description: 通用事件参数
 */

using System;

namespace UFramework
{
    public class EventParam : EventArgs, IStoppableEvent
    {
        private readonly object[] data;
        private bool isStopEvent;

        public EventParam(params object[] data)
        {
            this.data = data;
            isStopEvent = false;
        }

        public object[] Value => data;

        public bool IsStopEvent => isStopEvent;

        public EventParam StopEvent()
        {
            isStopEvent = true;
            return this;
        }

        public EventParam DeliverEvent()
        {
            isStopEvent = false;
            return this;
        }
    }
}