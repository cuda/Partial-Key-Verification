/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Computes a CRC-32 hash.
    /// This is a port of Michael Barr's public domain CRC code.
    /// See http://www.netrino.com/Connecting/2000-01/index.php
    /// </summary>
    public sealed class Crc32 : IHash, Checksum.IChecksum32
    {
        private static readonly uint[] CrcTable = CreateCrcTable();

        /// <summary>
        /// Compute a CRC-32 hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            var remainder = 0xFFFFFFFF;

            //Divide the message by the polynomial, a byte at a time.
            foreach (var t in data)
            {
                var index = (byte) (Reflect(t, 8) ^ (remainder >> 24));
                remainder = CrcTable[index] ^ (remainder << 8);
            }

            return Reflect(remainder, 32) ^ 0xFFFFFFFF;
        }

        private static uint Reflect(uint data, byte nBits)
        {
            uint reflection = 0;

            //Reflect the data about the center bit.
            for (byte bit = 0; bit < nBits; ++bit)
            {
                //If the LSB bit is set, set the reflection of it.
                if ((data & 0x01) != 0)
                {
                    reflection |= (uint) (1 << ((nBits - 1) - bit));
                }

                data = (data >> 1);
            }

            return reflection;
        }


        private static uint[] CreateCrcTable()
        {
            var table = new uint[256];
            const uint topbit = ((uint) 1 << 31);

            //Compute the remainder of each possible dividend.
            for (var dividend = 0; dividend < 256; ++dividend)
            {
                //Start with the dividend followed by zeros.
                var remainder = (uint) dividend << 24;

                //Perform modulo-2 division, a bit at a time.
                for (byte bit = 8; bit > 0; --bit)
                {
                    //Try to divide the current data bit.
                    if ((remainder & topbit) != 0)
                    {
                        remainder = (remainder << 1) ^ 0x04C11DB7;
                    }
                    else
                    {
                        remainder = remainder << 1;
                    }
                }

                //Store the result into the table.
                table[dividend] = remainder;
            }
            return table;
        }
    }
}