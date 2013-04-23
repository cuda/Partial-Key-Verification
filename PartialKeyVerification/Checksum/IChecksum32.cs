/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

namespace PartialKeyVerification.Checksum
{
    /// <summary>
    /// Classes that implement this interface should creates a 16-bit checksum of
    /// a given byte array.
    /// </summary>
    public interface IChecksum32
    {
        /// <summary>
        /// Compute a 32-bit checksum for the given byte array.
        /// </summary>
        /// <param name="data">The byte array to compute the checksum for.</param>
        /// <returns>The checksum for the byte array.</returns>
        uint Compute(byte[] data);
    }
}