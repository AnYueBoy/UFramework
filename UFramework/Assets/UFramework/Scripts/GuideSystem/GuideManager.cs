using System.Collections.Generic;

namespace UFramework
{
    public class GuideManager : IGuideManager
    {
        private IGuide curGuide;

        private Dictionary<GuideID, IGuide> allGuideDic = new Dictionary<GuideID, IGuide>()
        {
            { GuideID.Guide1, new Guide1(GuideID.Guide1) },
        };

        private GuideTrigger guideTrigger;
        private GuideMask guideMask;

        public void Init()
        {
        }

        public void LocalUpdate(float dt)
        {
            if (curGuide != null)
            {
                curGuide.LocalUpdate(dt);
                CheckGuideFinish();
            }

            CheckGuideTriggerCondition(dt);
        }

        public bool IsInGuiding()
        {
            return curGuide != null;
        }

        public bool IsInGuide(GuideID guideID)
        {
            return allGuideDic.ContainsKey(guideID);
        }

        private void CheckGuideFinish()
        {
            if (curGuide == null)
            {
                return;
            }

            if (curGuide.CurGuideState == GuideState.Completed)
            {
                allGuideDic.Remove(curGuide.CurGuideID);
                curGuide = null;
            }
        }

        private void CheckGuideTriggerCondition(float dt)
        {
            if (curGuide != null)
            {
                return;
            }

            var enumerator = allGuideDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current.Value;
                if (value.CheckTriggerCondition(dt))
                {
                    curGuide = value;
                    curGuide.InitDataForGuide();
                    curGuide.StartGuide();
                    break;
                }
            }
        }
    }
}