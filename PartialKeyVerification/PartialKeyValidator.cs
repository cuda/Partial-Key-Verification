/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;
using System.Text;
using PartialKeyVerification.Checksum;
using PartialKeyVerification.Hash;

namespace PartialKeyVerification
{
    /// <summary>
    /// This class will validate a Partial Key Verification key. A key is valid
    /// if the checksum is valid and the requested sub key(s) is(are) valid.
    /// </summary>
    public static class PartialKeyValidator
    {
        private static readonly IHash Hash = new Fnv1A();

        /// <summary>
        /// Validates the given key. Verifies the checksum and each sub key.
        /// </summary>
        /// <param name="hash">The hash algorithm used to compute the sub key.</param>
        /// <param name="checksum">The checksum algorithm used to compute the key's checksum.</param>
        /// <param name="key">The key to validate.</param>
        /// <param name="subkeyIndex">The index (zero based) of the sub key to check.</param>
        /// <param name="subkeyBase">The unsigned base integer used create the sub key.</param>
        /// <returns>true if the is valid; false otherwise.</returns>
        public static bool ValidateKey(IChecksum16 checksum, IHash hash, string key, int subkeyIndex, uint subkeyBase)
        {
            var bytes = GetKeyBytes(key);
            var seed = BitConverter.ToUInt32(bytes, 0);
            return ValidateKey(hash, checksum, bytes, seed, subkeyIndex, subkeyBase);
        }

        /// <summary>
        /// Validates the given key. Verifies the given string seed matches
        /// the seed embedded in the key, verifies the checksum and each sub key.
        /// This version is useful if the seed used to generate a key was derived
        /// from some user information such as the user's name, e-mail, etc.
        /// </summary>
        /// <param name="hash">The hash algorithm used to compute the sub key.</param>
        /// <param name="checksum">The checksum algorithm used to compute the key's checksum.</param>
        /// <param name="key">The key to validate.</param>
        /// <param name="subkeyIndex">The index (zero based) of the sub key to check.</param>
        /// <param name="subkeyBase">The unsigned base integer used create the sub key.</param>
        /// <param name="seedString">The string used to generate the seed for the key.</param>
        /// <returns>true if the is valid; false otherwise.</returns>
        public static bool ValidateKey(IChecksum16 checksum, IHash hash, string key, int subkeyIndex, uint subkeyBase,
                                       string seedString)
        {
            var bytes = GetKeyBytes(key);
            var seed = BitConverter.ToUInt32(bytes, 0);

            if (Hash.Compute(Encoding.UTF8.GetBytes(seedString)) != seed)
            {
                return false;
            }
            return ValidateKey(hash, checksum, bytes, seed, subkeyIndex, subkeyBase);
        }

        /// <summary>
        /// Extracts the serial number from a key.
        /// </summary>
        /// <param name="key">The key to extract the serial number from.</param>
        /// <returns>The serial number embedded in the key.</returns>
        public static uint GetSerialNumberFromKey(string key)
        {
            var bytes = GetKeyBytes(key);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a string seed into a serial number (uint seed).
        /// </summary>
        /// <param name="seed">The string seed to convert.</param>
        /// <returns>The string seed converted to a serial number.</returns>
        public static uint GetSerialNumberFromSeed(string seed)
        {
            return Hash.Compute(Encoding.UTF8.GetBytes(seed));
        }

        private static byte[] GetKeyBytes(string key)
        {
            //converts the sting key back into a byte array, removing
            //any separators.
            key = key.ToUpperInvariant();
            var pos = key.IndexOf('-');
            while (pos > -1)
            {
                key = key.Remove(pos, 1);
                pos = key.IndexOf('-');
            }

            return Base32.FromBase32(key);
        }

        //validates the checksum
        private static bool ValidateChecksum(IChecksum16 checksum, byte[] key)
        {
            var sum = BitConverter.ToUInt16(key, key.Length - 2);
            var keyBytes = new byte[key.Length - 2];
            Buffer.BlockCopy(key, 0, keyBytes, 0, keyBytes.Length);
            return sum == checksum.Compute(keyBytes);
        }

        //validates one sub key.
        private static bool ValidateKey(IHash hash, IChecksum16 checksum, byte[] key, uint seed, int subkeyIndex,
                                        uint subkeyBase)
        {
            if (!ValidateChecksum(checksum, key))
            {
                return false;
            }

            var offset = subkeyIndex*4 + 4;

            if (subkeyIndex < 0 || offset + 4 > key.Length - 2)
            {
                throw new IndexOutOfRangeException("Sub key index is out of bounds");
            }

            var subKey = BitConverter.ToUInt32(key, offset);
            var expected = hash.Compute(BitConverter.GetBytes(seed ^ subkeyBase));
            return expected == subKey;
        }
    }
}