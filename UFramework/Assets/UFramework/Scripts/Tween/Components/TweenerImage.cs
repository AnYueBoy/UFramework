using UnityEngine;

namespace UFramework.Tween
{
    public class TweenerImage<T> : Tweener<T>
    {
        public void FillAmountTweener(float dt, TweenerCore<float> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            float endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }
    }
}