/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// One-at-a-Time has as implemented by Bob Jenkins
    /// http://www.burtleburtle.net/bob/hash/doobs.html
    /// </summary>
    public sealed class OneAtATime : IHash
    {
        /// <summary>
        /// Compute a One-at-a-Tim hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        public uint Compute(byte[] data)
        {
            uint hash, i;
            for (hash = 0, i = 0; i < data.Length; ++i)
            {
                hash += data[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }
    }
}