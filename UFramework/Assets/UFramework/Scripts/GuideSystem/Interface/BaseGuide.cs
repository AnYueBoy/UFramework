namespace UFramework
{
    public abstract class BaseGuide
    {
        public virtual void InitDataForGuide()
        {
            CurGuideState = GuideState.Initialization;
        }

        public abstract bool CheckTriggerCondition(float dt);

        public virtual void StartGuide()
        {
            CurGuideState = GuideState.Running;
        }

        public abstract void LocalUpdate(float dt);

        protected abstract void FinishGuide();

        public GuideState CurGuideState { get; private set; }

        public GuideID CurGuideID { get; protected set; }
    }
}