/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Port of Bob Jenkins' implementation of Paul Hsieh's SuperFast hash.
    /// See http://www.burtleburtle.net/bob/hash/doobs.html
    /// </summary>
    public sealed class SuperFast : IHash
    {
        /// <summary>
        /// Compute a SuperFast hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The SuperFast hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            var len = data.Length;
            var hash = (uint) len;
            var rem = len & 3;
            len >>= 2;

            var offset = 0;
            /* Main loop */
            for (; len > 0; len--)
            {
                hash += BitConverter.ToUInt16(data, offset);
                offset += 2;
                var next = BitConverter.ToUInt16(data, offset);
                offset += 2;
                var tmp = (uint) (next << 11) ^ hash;
                hash = (hash << 16) ^ tmp;
                hash += hash >> 11;
            }

            /* Handle end cases */
            switch (rem)
            {
                case 3:
                    hash += BitConverter.ToUInt16(data, offset);
                    offset += 2;
                    hash ^= hash << 16;
                    hash ^= (uint) (data[offset] << 18);
                    hash += hash >> 11;
                    break;
                case 2:
                    hash += BitConverter.ToUInt16(data, offset);
                    hash ^= hash << 11;
                    hash += hash >> 17;
                    break;
                case 1:
                    hash += data[offset];
                    hash ^= hash << 10;
                    hash += hash >> 1;
                    break;
            }

            /* Force "avalanching" of final 127 bits */
            hash ^= hash << 3;
            hash += hash >> 5;
            hash ^= hash << 4;
            hash += hash >> 17;
            hash ^= hash << 25;
            hash += hash >> 6;

            return hash;
        }
    }
}