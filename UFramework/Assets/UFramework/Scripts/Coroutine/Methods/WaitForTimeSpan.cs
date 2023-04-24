using System;

namespace UFramework.Coroutine
{
    public class WaitForTimeSpan : YieldInstruction
    {
        private readonly DateTime _setTime;

        public WaitForTimeSpan(TimeSpan span) : base()
        {
            _setTime = DateTime.Now + span;
        }

        protected override bool IsCompleted()
        {
            return DateTime.Now >= _setTime;
        }
    }
}