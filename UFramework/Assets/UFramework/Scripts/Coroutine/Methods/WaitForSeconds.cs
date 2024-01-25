using System;

namespace UFramework
{
    public class WaitForSeconds : WaitForTimeSpan
    {
        public WaitForSeconds(double seconds) : base(TimeSpan.FromSeconds(seconds))
        {
        }
    }
}