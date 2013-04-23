/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;

namespace PartialKeyVerification.Checksum
{
    /// <summary>
    /// An implementation of the Adler-16 checksum.
    /// </summary>
    public sealed class Adler16 : IChecksum16
    {
        /// <summary>
        /// Compute the Adler-16 checksum for the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the checksum for.</param>
        /// <returns>The Adler-16 checksum for the byte array.</returns>
        public ushort Compute(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var a = 1u;
            var b = 0u;
            var len = data.Length;
            var offset = 0;
            while (len > 0)
            {
                var tlen = len < 5550 ? len : 5550;
                len -= tlen;

                do
                {
                    a += data[offset++];
                    b += a;
                } while (--tlen > 0);

                a %= 251;
                b %= 251;
            }
            return (ushort) ((b << 8) | a);
        }
    }
}