using System;

namespace UFramework.Coroutine
{
    public class WaitForSeconds : WaitForTimeSpan
    {
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
}