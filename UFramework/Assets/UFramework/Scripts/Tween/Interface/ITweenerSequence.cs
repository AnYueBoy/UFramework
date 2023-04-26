using System;

namespace UFramework.Tween
{
    public interface ITweenerSequence
    {
        void Init();

        ITweenerSequence Append(ITweener tweener);

        ITweenerSequence AppendInterval(float interval);

        ITweenerSequence AppendCallback(Action callback);
    }
}