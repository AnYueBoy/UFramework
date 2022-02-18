/*
 * @Author: l hy 
 * @Date: 2022-01-13 16:01:29 
 * @Description: 事件派发接口
 */

using System;
namespace UFramework.EventDispatcher {
    public interface IEventDispatcher {

        /// <summary>
        /// Add an event listener that listens on the specified events.
        /// </summary>
        bool AddListener (string eventName, EventHandler<EventParam> handler);

        /// <summary>
        ///  Remove an event listener from the specified events.
        /// </summary>
        bool RemoveListener (string eventName, EventHandler<EventParam> handler = null);

        /// <summary>
        /// Gets the listeners of a specific event or all listeners sorted by descending priority. Will not return listeners in the inheritance chain.
        /// </summary>
        EventHandler<EventParam>[] GetListeners (string eventName);

        /// <summary>
        ///  Whether an event has any registered event listener.
        /// </summary>
        bool HasListener (string eventName);

        /// <summary>
        /// Provide all relevant listeners with an event to process.
        /// </summary>
        void Raise (string eventName, object sender, EventParam e = null);
    }
}