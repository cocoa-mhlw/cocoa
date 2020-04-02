using System;

namespace UniversalBeacon.Library
{
    internal static class PacketExtensions
    {
        public static ulong ToNumericAddress(this string addressString)
        {
            ulong address = 0;
            var parts = addressString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            for(var i = 0; i < parts.Length; i++)
            {
                var b = (ulong)byte.Parse(parts[i], System.Globalization.NumberStyles.HexNumber);
                // they're in reverse order due to endianness
                address |= (b << ((parts.Length - i - 1) * 8));
            }

            return address;
        }
    }

}
