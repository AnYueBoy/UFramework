namespace UFramework
{
    public class Guide1 : BaseGuide
    {
        public Guide1(GuideID curGuideID)
        {
            CurGuideID = curGuideID;
        }


        public override bool CheckTriggerCondition(float dt)
        {
            throw new System.NotImplementedException();
        }

        public override void LocalUpdate(float dt)
        {
            throw new System.NotImplementedException();
        }

        protected override void FinishGuide()
        {
            throw new System.NotImplementedException();
        }
    }
}