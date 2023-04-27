using System;

namespace UFramework.Tween
{
    public class TweenerSequence : ITweenerSequence
    {
        private ITweener lastTweener;

        public TweenerSequence()
        {
            lastTweener = null;
        }

        public void Init()
        {
            lastTweener = null;
        }

        public ITweenerSequence Append(ITweener tweener)
        {
            if (lastTweener != null)
            {
                tweener.Pause();
                lastTweener.CompletedEvent += () =>
                {
                    tweener.UpdateGetterValue();
                    tweener.Resume();
                };
            }

            lastTweener = tweener;

            return this;
        }

        public ITweenerSequence AppendInterval(float interval)
        {
            var intervalTweener = TweenerExtension.TweenerTimer(interval);
            intervalTweener.Pause();
            lastTweener.CompletedEvent += () => { intervalTweener.Resume(); };
            lastTweener = intervalTweener;
            return this;
        }

        public ITweenerSequence AppendCallback(Action callback)
        {
            lastTweener.CompletedEvent += callback;
            return this;
        }
    }
}