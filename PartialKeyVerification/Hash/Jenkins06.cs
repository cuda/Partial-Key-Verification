/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Port of Bob Jenkins' Lookup3 hash function.
    /// http://www.burtleburtle.net/bob/hash/doobs.html
    /// </summary>
    public sealed class Jenkins06 : IHash
    {
        private readonly uint _seed;

        public Jenkins06(uint seed)
        {
            _seed = seed;
        }

        /// <summary>
        /// Compute a Lookup3 bit hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            uint a, b, c; /* internal state */

            var length = data.Length;

            /* Set up the internal state */
            a = b = c = 0xdeadbeef + ((uint) length) + _seed;

            /*--------------- all but the last block: affect some 32 bits of (a,b,c) */
            var offset = 0;
            while (length > 12)
            {
                a += data[offset++];
                a += ((uint) data[offset++]) << 8;
                a += ((uint) data[offset++]) << 16;
                a += ((uint) data[offset++]) << 24;
                b += data[offset++];
                b += ((uint) data[offset++]) << 8;
                b += ((uint) data[offset++]) << 16;
                b += ((uint) data[offset++]) << 24;
                c += data[offset++];
                c += ((uint) data[offset++]) << 8;
                c += ((uint) data[offset++]) << 16;
                c += ((uint) data[offset++]) << 24;
                Mix(ref a, ref b, ref c);
                length -= 12;
            }

            /*-------------------------------- last block: affect all 32 bits of (c) */
            switch (length) /* all the case statements fall through */
            {
                case 12:
                    c += ((uint) data[offset++]) << 24;
                    goto case 11;
                case 11:
                    c += ((uint) data[offset++]) << 16;
                    goto case 10;
                case 10:
                    c += ((uint) data[offset++]) << 8;
                    goto case 9;
                case 9:
                    c += data[offset++];
                    goto case 8;
                case 8:
                    b += ((uint) data[offset++]) << 24;
                    goto case 7;
                case 7:
                    b += ((uint) data[offset++]) << 16;
                    goto case 6;
                case 6:
                    b += ((uint) data[offset++]) << 8;
                    goto case 5;
                case 5:
                    b += data[offset++];
                    goto case 4;
                case 4:
                    a += ((uint) data[offset++]) << 24;
                    goto case 3;
                case 3:
                    a += ((uint) data[offset++]) << 16;
                    goto case 2;
                case 2:
                    a += ((uint) data[offset++]) << 8;
                    goto case 1;
                case 1:
                    a += data[offset];
                    break;
                case 0:
                    return c;
            }
            Final(ref a, ref b, ref c);
            return c;
        }

        private static uint Rot(uint x, int k)
        {
            return ((x << k) | (x >> (32 - k)));
        }

        private static void Final(ref uint a, ref uint b, ref uint c)
        {
            c ^= b;
            c -= Rot(b, 14);
            a ^= c;
            a -= Rot(c, 11);
            b ^= a;
            b -= Rot(a, 25);
            c ^= b;
            c -= Rot(b, 16);
            a ^= c;
            a -= Rot(c, 4);
            b ^= a;
            b -= Rot(a, 14);
            c ^= b;
            c -= Rot(b, 24);
        }

        private static void Mix(ref uint a, ref uint b, ref uint c)
        {
            a -= c;
            a ^= Rot(c, 4);
            c += b;
            b -= a;
            b ^= Rot(a, 6);
            a += c;
            c -= b;
            c ^= Rot(b, 8);
            b += a;
            a -= c;
            a ^= Rot(c, 16);
            c += b;
            b -= a;
            b ^= Rot(a, 19);
            a += c;
            c -= b;
            c ^= Rot(b, 4);
            b += a;
        }
    }
}