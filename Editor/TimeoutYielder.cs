using System;
using UnityEngine;

namespace TypeCodebase
{
    public class TimeoutYielder
    {
        public long TimeoutMilliseconds { get; set; }

        private long _startTime;

        public TimeoutYielder(long timeoutMilliseconds)
        {
            Reset();
            TimeoutMilliseconds = timeoutMilliseconds;
        }

        public bool ShouldYield()
        {
            bool yieldThisFrame = DateTimeOffset.Now.ToUnixTimeMilliseconds() > _startTime + TimeoutMilliseconds;
            if (yieldThisFrame)
            {
                Reset();
            }
            return yieldThisFrame;
        }

        public void Reset()
        {
            _startTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}