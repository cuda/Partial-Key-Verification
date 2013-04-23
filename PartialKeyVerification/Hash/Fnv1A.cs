/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Implementation of the FNV1a hash functions.
    /// See http://isthe.com/chongo/tech/comp/fnv/ for details.
    /// </summary>
    public sealed class Fnv1A : IHash
    {
        /// <summary>
        /// Compute a 32 bit hash of the given byte array using FNV1a algorithm.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var hval = 2166136261;
            foreach (var t in data)
            {
                hval ^= t;
                hval += (hval << 1) + (hval << 4) + (hval << 7) + (hval << 8) + (hval << 24);
            }
            return hval;
        }
    }
}