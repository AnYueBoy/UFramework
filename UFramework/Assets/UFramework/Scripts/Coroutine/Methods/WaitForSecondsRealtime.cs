using UnityEngine;

namespace UFramework
{
    public class WaitForSecondsRealtime : WaitForSeconds
    {
        public WaitForSecondsRealtime(float seconds) : base(seconds)
        {
        }

        protected override bool IsCompleted()
        {
            timer += Time.unscaledDeltaTime;
            return timer >= interval;
        }
    }
}