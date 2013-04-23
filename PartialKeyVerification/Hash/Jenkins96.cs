/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Implements the Bob Jenkins' Lookup2 hash function.
    /// See http://www.burtleburtle.net/bob/hash/doobs.html for more details.
    /// </summary>
    public sealed class Jenkins96 : IHash
    {
        private uint _a, _b, _c;

        /// <summary>
        /// Compute a 32 bit hash of the given byte array using the Bob Jenkins' Lookup2 algorithm.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            var len = data.Length;
            _a = _b = 0x9e3779b9;
            _c = 0;
            var i = 0;
            while (i + 12 <= len)
            {
                _a += data[i++] |
                      ((uint) data[i++] << 8) |
                      ((uint) data[i++] << 16) |
                      ((uint) data[i++] << 24);
                _b += data[i++] |
                      ((uint) data[i++] << 8) |
                      ((uint) data[i++] << 16) |
                      ((uint) data[i++] << 24);
                _c += data[i++] |
                      ((uint) data[i++] << 8) |
                      ((uint) data[i++] << 16) |
                      ((uint) data[i++] << 24);
                Mix();
            }
            _c += (uint) len;
            if (i < len)
            {
                _a += data[i++];
            }
            if (i < len)
            {
                _a += (uint) data[i++] << 8;
            }
            if (i < len)
            {
                _a += (uint) data[i++] << 16;
            }
            if (i < len)
            {
                _a += (uint) data[i++] << 24;
            }
            if (i < len)
            {
                _b += data[i++];
            }
            if (i < len)
            {
                _b += (uint) data[i++] << 8;
            }
            if (i < len)
            {
                _b += (uint) data[i++] << 16;
            }
            if (i < len)
            {
                _b += (uint) data[i++] << 24;
            }
            if (i < len)
            {
                _c += (uint) data[i++] << 8;
            }
            if (i < len)
            {
                _c += (uint) data[i++] << 16;
            }
            if (i < len)
            {
                _c += (uint) data[i] << 24;
            }
            Mix();
            return _c;
        }

        private void Mix()
        {
            _a -= _b;
            _a -= _c;
            _a ^= (_c >> 13);
            _b -= _c;
            _b -= _a;
            _b ^= (_a << 8);
            _c -= _a;
            _c -= _b;
            _c ^= (_b >> 13);
            _a -= _b;
            _a -= _c;
            _a ^= (_c >> 12);
            _b -= _c;
            _b -= _a;
            _b ^= (_a << 16);
            _c -= _a;
            _c -= _b;
            _c ^= (_b >> 5);
            _a -= _b;
            _a -= _c;
            _a ^= (_c >> 3);
            _b -= _c;
            _b -= _a;
            _b ^= (_a << 10);
            _c -= _a;
            _c -= _b;
            _c ^= (_b >> 15);
        }
    }
}