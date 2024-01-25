using System;

namespace UFramework
{
    public class WaitUtil : YieldInstruction
    {
        private Func<bool> _condition;

        public WaitUtil(Func<bool> condition)
        {
            _condition = condition;
        }

        protected override bool IsCompleted()
        {
            return _condition.Invoke();
        }
    }
}