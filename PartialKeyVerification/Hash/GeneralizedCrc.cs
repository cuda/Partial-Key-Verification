/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Port of Bob Jenkins Generalized CRC code.
    /// http://www.burtleburtle.net/bob/c/gencrc.c
    /// </summary>
    public class GeneralizedCrc : IHash
    {
        private static readonly uint[] CrcTable = CreateCrcTable();

        /// <summary>
        /// Compute a Generalized CRC hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            var len = (uint) data.Length;
            uint hash, i;
            for (hash = len, i = 0; i < len; ++i)
            {
                hash = (hash >> 8) ^ CrcTable[(hash & 0xff) ^ data[i]];
            }
            return hash;
        }

        private static uint[] CreateCrcTable()
        {
            var table = new uint[256];

            /* fill tab[] with random permutations of 0..255 in each byte */
            for (uint i = 0; i < 256; i ++)
            {
                var x = (byte) i;
                for (uint j = 0; j < 5; ++j)
                {
                    x += 1;
                    x += (byte) (x << 1);
                    x ^= (byte) (x >> 1);
                }
                table[i] = x;
                for (uint j = 0; j < 5; ++j)
                {
                    x += 2;
                    x += (byte) (x << 1);
                    x ^= (byte) (x >> 1);
                }
                table[i] ^= (uint) (x << 8);
                for (uint j = 0; j < 5; ++j)
                {
                    x += 3;
                    x += (byte) (x << 1);
                    x ^= (byte) (x >> 1);
                }
                table[i] ^= ((uint) x << 16);
                for (uint j = 0; j < 5; ++j)
                {
                    x += 4;
                    x += (byte) (x << 1);
                    x ^= (byte) (x >> 1);
                }
                table[i] ^= (uint) (x << 24);
            }
            return table;
        }
    }
}