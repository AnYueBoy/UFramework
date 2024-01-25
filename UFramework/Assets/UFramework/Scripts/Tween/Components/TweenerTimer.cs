using UnityEngine;

namespace UFramework
{
    public class TweenerTimer<T> : Tweener<T>
    {
        public void TimerTween(float dt, TweenerCore<float> tweenerCore)
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