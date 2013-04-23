/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Hash
{
    /// <summary>
    /// Classes that implement this interface should create a 32 bit hash
    /// of the given byte array.
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// Compute a 32 bit hash of the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the hash from.</param>
        /// <returns>The hash for the byte array.</returns>
        uint Compute(byte[] data);
    }
}