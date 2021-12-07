using System;
using System.Numerics;
using System.Security.Cryptography;
using Aprismatic.PohligHellman;
using Xunit;
using Xunit.Abstractions;

namespace ElGamalTests
{
    public class EdgeCases : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private readonly Random _rnd = new();
        private readonly RandomNumberGenerator _rng = new RNGCryptoServiceProvider();

        public EdgeCases(ITestOutputHelper output) {
            _output = output;
        }

        public void Dispose() {
            _rng.Dispose();
        }

        [Fact(DisplayName = "Zero")]
        public void TestZero() {
            for (var i = 0; i < Globals.Iterations; i++) {
                using var algorithm = new PohligHellman(_rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength));

                var z = BigInteger.Zero;

                var z_enc = algorithm.EncryptData(z);
                var z_dec = algorithm.DecryptData(z_enc);

                Assert.Equal(z, z_dec);
            }
        }

        [Fact(DisplayName = "One")]
        public void TestOne() {
            for (var i = 0; i < Globals.Iterations; i++) {
                using var algorithm = new PohligHellman(_rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength));

                var o = BigInteger.One;

                var o_enc = algorithm.EncryptData(o);
                var o_dec = algorithm.DecryptData(o_enc);

                Assert.Equal(o, o_dec);
            }
        }

        [Fact(DisplayName = "Edge values")]
        public void MinAndMaxValues() {
            for (var i = 0; i < Globals.Iterations; i++) {
                using var algorithm = new PohligHellman(_rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength));

                var max = algorithm.MaxEncryptableValue; // should work
                var max_plus = max + 1; // should throw
                var min = 0; // should work
                var min_minus = min - 1; // should throw

                // MAX
                var max_enc = algorithm.EncryptData(max);
                var max_dec = algorithm.DecryptData(max_enc);
                Assert.True(max_dec == max, $"{Environment.NewLine}{Environment.NewLine}" +
                                            $"Algorithm parameters (TRUE):{Environment.NewLine}" +
                                            $"{algorithm.ToXmlString()}{Environment.NewLine}{Environment.NewLine}" +
                                            $"max     : {max}{Environment.NewLine}{Environment.NewLine}" +
                                            $"max_dec : {max_dec}");

                // MIN
                var min_enc = algorithm.EncryptData(min);
                var min_dec = algorithm.DecryptData(min_enc);
                Assert.True(min_dec == min, $"{Environment.NewLine}{Environment.NewLine}" +
                                            $"Algorithm parameters (TRUE):{Environment.NewLine}" +
                                            $"{algorithm.ToXmlString()}{Environment.NewLine}{Environment.NewLine}" +
                                            $"min     : {min}{Environment.NewLine}{Environment.NewLine}" +
                                            $"min_dec : {min_dec}");

                // MAX + 1
                Assert.Throws<ArgumentException>(() => algorithm.EncryptData(max_plus));

                // MIN - 1
                Assert.Throws<ArgumentException>(() => algorithm.EncryptData(min_minus));
            }
        }
    }
}
