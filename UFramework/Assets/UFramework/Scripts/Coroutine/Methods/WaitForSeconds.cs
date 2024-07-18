using System;
using UnityEngine;

namespace UFramework
{
    public class WaitForSeconds : YieldInstruction
    {
        protected float timer;
        protected readonly float interval;

        public WaitForSeconds(float seconds)
        {
            timer = 0;
            interval = seconds;
        }

        protected override bool IsCompleted()
        {
            timer += Time.deltaTime;
            return timer >= interval;
        }
    }
}