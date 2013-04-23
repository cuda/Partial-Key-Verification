/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Checksum
{
    /// <summary>
    /// Computes a CRC-CCITT checksum.
    /// This is a port of Michael Barr's public domain CRC code.
    /// See http://www.netrino.com/Connecting/2000-01/index.php
    /// </summary>
    public sealed class CrcCcitt : IChecksum16
    {
        private static readonly ushort[] CrcTable = CreateCrcTable();


        /// <summary>
        /// Compute a CRC-CCITT checksum for the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the checksum for.</param>
        /// <returns>The checksum for the byte array.</returns>
        public ushort Compute(byte[] data)
        {
            ushort remainder = 0xFFFF;

            //Divide the message by the polynomial, a byte at a time.
            foreach (var t in data)
            {
                var index = (byte) (t ^ (remainder >> 8));
                remainder = (ushort) (CrcTable[index] ^ (remainder << 8));
            }

            return remainder;
        }

        private static ushort[] CreateCrcTable()
        {
            var table = new ushort[256];
            const ushort topbit = 1 << 15;

            //Compute the remainder of each possible dividend.
            for (var dividend = 0; dividend < 256; ++dividend)
            {
                //Start with the dividend followed by zeros.
                var remainder = (ushort) (dividend << 8);

                //Perform modulo-2 division, a bit at a time.
                for (byte bit = 8; bit > 0; --bit)
                {
                    //Try to divide the current data bit.
                    if ((remainder & topbit) != 0)
                    {
                        remainder = (ushort) ((remainder << 1) ^ 0x1021);
                    }
                    else
                    {
                        remainder = (ushort) (remainder << 1);
                    }
                }

                //Store the result into the table.
                table[dividend] = remainder;
            }
            return table;
        }
    }
}