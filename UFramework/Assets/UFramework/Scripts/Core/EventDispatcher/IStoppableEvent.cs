/*
 * @Author: l hy
 * @Date: 2022-01-13 16:42:57
 * @Description: 停止事件派发接口
 */

namespace UFramework
{
    public interface IStoppableEvent
    {
        bool IsStopEvent { get; }
        EventParam StopEvent();

        EventParam DeliverEvent();
    }
}