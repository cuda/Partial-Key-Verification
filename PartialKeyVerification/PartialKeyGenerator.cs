/* Copyright 2007-2013 Marcus Cuda - http://marcuscuda.com
 * 
 * Licensed under the Simplified BSD license - see LICENSE.txt
 */

using System;
using System.Collections.Generic;
using System.Text;
using PartialKeyVerification.Checksum;
using PartialKeyVerification.Hash;

namespace PartialKeyVerification
{
    /// <summary>
    /// A Partial Key Verification key generator.
    /// This implementation allows you to specify the checksum and hash algorithms
    /// to use, and what license data to incorporated into the key.
    /// </summary>
    public sealed class PartialKeyGenerator
    {
        private static readonly IHash Hash = new Fnv1A();
        private readonly uint[] _baseKeys;
        private readonly IChecksum16 _checksum;
        private readonly IList<IHash> _hashFunctions = new List<IHash>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialKeyGenerator"/> class.
        /// </summary>
        /// <param name="checksum">The checksum algorithm to use.</param>
        /// <param name="hash">The hash algorithm to use.</param>
        /// <param name="baseKeys">The integer bases keys used to generate the sub keys (one base key for each sub key).</param>
        public PartialKeyGenerator(IChecksum16 checksum, IHash hash, uint[] baseKeys)
            : this(checksum, new[] {hash}, baseKeys)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialKeyGenerator"/> class.
        /// </summary>
        /// <param name="checksum">The checksum algorithm to use.</param>
        /// <param name="hashFunctions">A list of hash functions to use. If the number of
        /// hash functions is less than the number <paramref name="baseKeys"/>, then the 
        /// functions cycles back to the first function.  It is recommended to use several
        /// different hash functions.</param>
        /// <param name="baseKeys">The integers used to generate the sub key.</param>
        public PartialKeyGenerator(IChecksum16 checksum, IEnumerable<IHash> hashFunctions, uint[] baseKeys)
        {
            if (checksum == null)
            {
                throw new ArgumentNullException("checksum");
            }
            if (hashFunctions == null)
            {
                throw new ArgumentNullException("hashFunctions");
            }
            if (baseKeys == null)
            {
                throw new ArgumentNullException("baseKeys");
            }

            _checksum = checksum;
            _baseKeys = baseKeys;

            foreach (var hash in hashFunctions)
            {
                _hashFunctions.Add(hash);
            }
        }

        /// <summary>
        /// Gets or sets the spacing of the key separator.
        /// </summary>
        /// <value>The spacing of the key seperator.</value>
        public byte Spacing { get; set; }

        /// <summary>
        /// Generate a key based on the given seed.
        /// </summary>
        /// <param name="seed">The seed value to generate the key from.</param>
        /// <returns>A licensing key.</returns>
        public string Generate(uint seed)
        {
            var data = new byte[(_baseKeys.Length*4) + 4];

            // add seed
            data[0] = (byte) (seed & 0xFF);
            data[1] = (byte) (seed >> 8);
            data[2] = (byte) (seed >> 16);
            data[3] = (byte) (seed >> 24);

            //which hash function to use
            var hashOffset = 0;
            for (var i = 0; i < _baseKeys.Length; i++)
            {
                var digit = seed ^ _baseKeys[i];
                var hval = _hashFunctions[hashOffset++].Compute((BitConverter.GetBytes(digit)));
                data[4 + (4*i)] = (byte) (hval & 0xFF);
                data[5 + (4*i)] = (byte) (hval >> 8);
                data[6 + (4*i)] = (byte) (hval >> 16);
                data[7 + (4*i)] = (byte) (hval >> 24);

                hashOffset %= _hashFunctions.Count;
            }

            var checksum = _checksum.Compute(data);
            var key = new byte[data.Length + 2];
            Buffer.BlockCopy(data, 0, key, 0, data.Length);
            key[key.Length - 2] = (byte) (checksum & 0xFF);
            key[key.Length - 1] = (byte) (checksum >> 8);

            var ret = Base32.ToBase32(key);
            if (Spacing > 0)
            {
                var count = (ret.Length/Spacing);
                if (ret.Length%Spacing == 0)
                {
                    --count;
                }

                for (var i = 0; i < count; i++)
                {
                    ret = ret.Insert(Spacing + (i*Spacing + i), "-");
                }
            }
            return ret;
        }

        /// <summary>
        /// Generate a key based on the given string seed. Generate will hash the
        /// given string to create a uint seed.
        /// </summary>
        /// <param name="seed">The seed value to generate the key from.</param>
        /// <returns>A licensing key.</returns>
        public string Generate(string seed)
        {
            return Generate(Hash.Compute(Encoding.UTF8.GetBytes(seed)));
        }

        /// <summary>
        /// Generates a set of random keys.
        /// </summary>
        /// <param name="numberOfKeys">The number of keys to generate.</param>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A set of randomly generate keys.</returns>
        public IDictionary<uint, string> Generate(uint numberOfKeys, Random random)
        {
            var keys = new Dictionary<uint, string>();
            for (var i = 0; i < numberOfKeys; i++)
            {
                var bytes = new byte[4];
                random.NextBytes(bytes);
                var seed = BitConverter.ToUInt32(bytes, 0);
                
                // make sure we don't create duplicate keys
                while (keys.ContainsKey(seed))
                {
                    random.NextBytes(bytes);
                    seed = BitConverter.ToUInt32(bytes, 0);
                }

                keys.Add(seed, Generate(seed));
            }
            return keys;
        }
    }
}