using System.Collections.Generic;

namespace UFramework
{
    public class GuideManager : IGuideManager
    {
        private BaseGuide curBaseGuide;

        private Dictionary<GuideID, BaseGuide> allGuideDic = new Dictionary<GuideID, BaseGuide>()
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
            if (curBaseGuide != null)
            {
                curBaseGuide.LocalUpdate(dt);
                CheckGuideFinish();
            }

            CheckGuideTriggerCondition(dt);
        }

        public bool IsInGuiding()
        {
            return curBaseGuide != null;
        }

        public bool IsInGuide(GuideID guideID)
        {
            return allGuideDic.ContainsKey(guideID);
        }

        private void CheckGuideFinish()
        {
            if (curBaseGuide == null)
            {
                return;
            }

            if (curBaseGuide.CurGuideState == GuideState.Completed)
            {
                allGuideDic.Remove(curBaseGuide.CurGuideID);
                curBaseGuide = null;
            }
        }

        private void CheckGuideTriggerCondition(float dt)
        {
            if (curBaseGuide != null)
            {
                return;
            }

            var enumerator = allGuideDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current.Value;
                if (value.CheckTriggerCondition(dt))
                {
                    curBaseGuide = value;
                    curBaseGuide.InitDataForGuide();
                    curBaseGuide.StartGuide();
                    break;
                }
            }
        }
    }
}