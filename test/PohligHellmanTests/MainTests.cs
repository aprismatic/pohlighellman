using System;
using System.Numerics;
using System.Security.Cryptography;
using Aprismatic;
using Aprismatic.PohligHellman;
using Xunit;
using Xunit.Abstractions;

namespace PohligHellmanTests
{
    public class MainTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private readonly Random _rnd = new();
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public MainTests(ITestOutputHelper output) {
            _output = output;
        }

        public void Dispose() {
            _rng.Dispose();
        }

        [Fact(DisplayName = "Correctness")]
        public void TestCorrectness() {
            for (var i = 0; i < Globals.Iterations; i++) {
                var ks = _rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength);
                using var algorithm = new PohligHellman(ks);

                Assert.Equal(ks, algorithm.KeySize);

                var o = BigInteger.Zero.GenRandomBits(1, BigInteger.Pow(2, algorithm.KeySize - 1) - 1, _rng);

                var o_enc = algorithm.EncryptData(o);
                var o_dec = algorithm.DecryptData(o_enc);

                Assert.Equal(o, o_dec);
            }

            for (var i = 0; i < Globals.Iterations; i++) {
                var p = BigInteger.One.GenPseudoPrime(_rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength), 16, _rng);
                using var algorithm = new PohligHellman(p);

                Assert.Equal(p, algorithm.P);

                var o = BigInteger.Zero.GenRandomBits(1, BigInteger.Pow(2, algorithm.KeySize - 1) - 1, _rng);

                var o_enc = algorithm.EncryptData(o);
                var o_dec = algorithm.DecryptData(o_enc);

                Assert.Equal(o, o_dec);
            }
        }
    }
}
