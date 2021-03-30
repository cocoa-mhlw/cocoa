/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

﻿using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace Covid19Radar.Api.Common
{
    public class PartitionKeyRotation
    {
        private readonly KeyInformation[] Keys;
        private readonly int Max;
        private int Current = -1;
        public PartitionKeyRotation(string[] keys)
        {
            Keys = keys.Select((_, i) => new KeyInformation(_, i + 1)).ToArray();
            Max = keys.Length;
            Current = RandomNumberGenerator.GetInt32(0, Max - 1);
        }

        public KeyInformation Next()
        {
            var nextValue = Interlocked.Increment(ref Current);
            var nextKey = Keys[nextValue % Max];
            if (nextValue >= Max)
            {
                Interlocked.CompareExchange(ref Current, nextValue % Max, nextValue);
            }
            return nextKey;
        }

        public int Increment => Keys.Length;

        public class KeyInformation
        {
            private string _Self;
            public KeyInformation(string key, int initialValue)
            {
                Key = key;
                InitialValue = (ulong)initialValue;
            }
            public string Key { get; }
            public string Self { get => _Self; }
            public void SetSelf(string self)
            {
                Interlocked.CompareExchange(ref _Self, self, null);
            }
            public readonly ulong InitialValue;
        }
    }
}
