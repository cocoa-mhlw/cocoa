using System;
using System.Collections.Generic;
using System.Text;

namespace Covid19Radar.Common
{
    public class JumpHash
    {
        private const ulong constant = 2862933555777941757L;
        private const long constant2 = 1L << 31;

        public static int JumpConsistentHash(object key, int buckets)
        {
            return JumpConsistentHash((ulong)key.GetHashCode(), buckets);
        }
        public static int JumpConsistentHash(ulong key, int buckets)
        {

            long b = -1, j = 0;
            while (j < buckets)
            {
                b = j;

                key = key * constant + 1;
                ulong keyShift = key >> 33 + 1;

                j = (long)((b + 1) * ((double)constant2 / (double)keyShift));
            }

            return (int)b;
        }
    }
}
