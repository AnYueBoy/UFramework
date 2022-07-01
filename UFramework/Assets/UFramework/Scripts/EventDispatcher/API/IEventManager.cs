namespace UFramework.EventDispatcher
{
    public interface IEventManager
    {
        public event CustomEvent HideEvent
        {
            add { }
            remove { }
        }

        public event CustomEvent ShowEvent
        {
            add { }
            remove { }
        }

        public void DispatcherHideEvent();
        public void DispatcherShowEvent();
    }
}