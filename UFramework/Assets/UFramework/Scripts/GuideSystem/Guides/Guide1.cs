namespace UFramework
{
    public class Guide1 : IGuide
    {
        public void InitDataForGuide()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckTriggerCondition(float dt)
        {
            throw new System.NotImplementedException();
        }

        public void StartGuide()
        {
            throw new System.NotImplementedException();
        }

        public void LocalUpdate(float dt)
        {
            throw new System.NotImplementedException();
        }

        public void FinishGuide()
        {
            throw new System.NotImplementedException();
        }

        public GuideState CurGuideState { get; private set; }

        public GuideID CurGuideID { get; private set; }

        public Guide1(GuideID curGuideID)
        {
            CurGuideID = curGuideID;
        }
    }
}