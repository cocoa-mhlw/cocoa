using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Covid19Radar.Api.Common
{
    public class PartitionKeyRotation
    {
        private readonly string[] Keys;
        private readonly int Max;
        private int Current = -1;
        public PartitionKeyRotation(string[] keys)
        {
            Keys = keys;
            Max = keys.Length;
            Current = RandomNumberGenerator.GetInt32(0, Max - 1);
        }

        public string Next()
        {
            var nextValue = Interlocked.Increment(ref Current);
            var nextKey = Keys[nextValue % Max];
            if (nextValue >= Max)
            {
                Interlocked.CompareExchange(ref Current, nextValue % Max, nextValue);
            }
            return nextKey;
        }

    }
}
