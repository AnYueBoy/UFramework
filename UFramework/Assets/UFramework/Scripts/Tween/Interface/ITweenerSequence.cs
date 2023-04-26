using System;

namespace UFramework.Tween
{
    public interface ITweenerSequence
    {
        ITweenerSequence Append(ITweener tweener);

        ITweenerSequence AppendInterval(float interval);

        ITweenerSequence AppendCallback(Action callback);

    }
}