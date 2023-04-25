using UnityEngine;

namespace UFramework.Tween
{
    public class TweenerValue<T> : Tweener<T>
    {
        public void ValueTween(float dt, TweenerCore<float> tweenerCore)
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

        public void ValueTween(float dt, TweenerCore<Vector3> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector3 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }
        
        public void ValueTween(float dt, TweenerCore<Vector2> tweenerCore)
        {
            timer += dt;
            if (timer > tweenerCore.duration)
            {
                TriggerCompleted();
                return;
            }

            float time = Mathf.Min(tweenerCore.duration, timer);
            float ratioValue = EaseManager.GetEaseFuncValue(tweenerCore.easeType, time, tweenerCore.duration);
            Vector2 endValue = tweenerCore.changeValue * ratioValue + tweenerCore.beginValue;
            tweenerCore.setter(endValue);
        }
    }
}