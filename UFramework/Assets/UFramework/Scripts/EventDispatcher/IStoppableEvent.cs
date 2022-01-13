/*
 * @Author: l hy 
 * @Date: 2022-01-13 16:42:57 
 * @Description: 停止事件派发接口
 */

namespace UFramework.EventDispatcher {

    public interface IStoppableEvent {

        bool IsPropagationStopped { get; }
    }
}