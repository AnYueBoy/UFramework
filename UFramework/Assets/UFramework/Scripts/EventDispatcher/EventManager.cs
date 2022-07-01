namespace UFramework.EventDispatcher
{
    public class EventManager : IEventManager
    {
        private event CustomEvent hideEvent;
        private event CustomEvent showEvent;

        public EventManager()
        {
        }

        public event CustomEvent HideEvent
        {
            add => hideEvent += value;
            remove => hideEvent -= value;
        }

        public event CustomEvent ShowEvent
        {
            add => showEvent += value;
            remove => showEvent -= value;
        }

        public void DispatcherHideEvent()
        {
            hideEvent?.Invoke();
        }

        public void DispatcherShowEvent()
        {
            showEvent?.Invoke();
        }
    }

    public delegate void CustomEvent();
}