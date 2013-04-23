using System;
using NUnit.Framework;
using PartialKeyVerification;
using PartialKeyVerification.Checksum;
using PartialKeyVerification.Hash;

namespace PartialKeyTests
{
    [TestFixture]
    internal class VerificationTest
    {
        private static readonly Random Random = new Random(0);

        private static readonly IHash[] TestHashes = new IHash[]
            {
                new Fnv1A(), new Jenkins96(), new Crc32(), new GeneralizedCrc(),
                new Jenkins06((uint) Random.Next()), new OneAtATime(), new SuperFast()
            };

        [Test, TestCaseSource("TestHashes")]
        public void GenerateListOfKeys(IHash hash)
        {
            IChecksum16 checksum = new Adler16();
            var baseKeys = new[]
                {
                    (uint) Random.Next(),
                    (uint) Random.Next(),
                    (uint) Random.Next(),
                    (uint) Random.Next(),
                    (uint) Random.Next()
                };
            var gen = new PartialKeyGenerator(checksum, hash, baseKeys) {Spacing = 6};

            var keys = gen.Generate(100, Random);
            foreach (var key in keys)
            {
                for (var j = 0; j < baseKeys.Length; j++)
                {
                    Assert.IsTrue(PartialKeyValidator.ValidateKey(checksum, hash, key.Value, j, baseKeys[j]));
                }
            }
        }

        [Test]
        public void MultipleHashes()
        {
            IChecksum16 checksum = new Adler16();

            for (uint i = 0; i < 100; i++)
            {
                var baseKeys = new[]
                    {
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next()
                    };

                var gen = new PartialKeyGenerator(checksum, TestHashes, baseKeys) {Spacing = 6};

                var seed = (uint) Random.Next();
                var key = gen.Generate(seed);

                var hashIndex = 0;
                for (var j = 0; j < baseKeys.Length; j++)
                {
                    Assert.IsTrue(PartialKeyValidator.ValidateKey(checksum, TestHashes[hashIndex++], key, j, baseKeys[j]));
                    hashIndex %= TestHashes.Length;
                }
            }
        }

        [Test]
        public void MultipleHashesWithStringSeed()
        {
            IChecksum16 checksum = new Adler16();

            for (uint i = 0; i < 100; i++)
            {
                var baseKeys = new[]
                    {
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next()
                    };

                var gen = new PartialKeyGenerator(checksum, TestHashes, baseKeys) {Spacing = 6};

                const string seed = "Bob Smith - bob@smith.com";
                var key = gen.Generate(seed);

                var hashIndex = 0;
                for (var j = 0; j < baseKeys.Length; j++)
                {
                    Assert.IsTrue(PartialKeyValidator.ValidateKey(checksum, TestHashes[hashIndex++], key, j, baseKeys[j],
                                                                  seed));
                    hashIndex %= TestHashes.Length;
                }
            }
        }

        [Test]
        public void RetrieveSerialNumber()
        {
            var generator = new PartialKeyGenerator(new Adler16(), new Jenkins96(), new uint[] {1, 2, 3, 4})
                {
                    Spacing = 6
                };
            const string stringSeed = "bob@smith.com";
            var key = generator.Generate(stringSeed);
            var serialNumber = PartialKeyValidator.GetSerialNumberFromKey(key);
            Assert.AreEqual(serialNumber, PartialKeyValidator.GetSerialNumberFromSeed(stringSeed));
        }

        [Test, TestCaseSource("TestHashes")]
        public void SingleHash(IHash hash)
        {
            IChecksum16 checksum = new Adler16();

            for (uint i = 0; i < 100; i++)
            {
                var baseKeys = new[]
                    {
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next()
                    };
                var gen = new PartialKeyGenerator(checksum, hash, baseKeys) {Spacing = 6};

                var seed = (uint) Random.Next();
                var key = gen.Generate(seed);

                for (var j = 0; j < baseKeys.Length; j++)
                {
                    Assert.IsTrue(PartialKeyValidator.ValidateKey(checksum, hash, key, j, baseKeys[j]));
                }
            }
        }

        [Test, TestCaseSource("TestHashes")]
        public void SingleHashWithStringSeed(IHash hash)
        {
            IChecksum16 checksum = new Adler16();

            for (uint i = 0; i < 100; i++)
            {
                var baseKeys = new[]
                    {
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next(),
                        (uint) Random.Next()
                    };
                var gen = new PartialKeyGenerator(checksum, hash, baseKeys) {Spacing = 6};

                const string seed = "Bob Smith - bob@smith.com";
                var key = gen.Generate(seed);

                for (var j = 0; j < baseKeys.Length; j++)
                {
                    Assert.IsTrue(PartialKeyValidator.ValidateKey(checksum, hash, key, j, baseKeys[j], seed));
                }
            }
        }
    }
}