using System;
using System.Numerics;
using Aprismatic;
using Aprismatic.PohligHellman;
using Xunit;
using Xunit.Abstractions;

namespace PohligHellmanTests
{
    public class KeyStruct
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _rnd;

        public KeyStruct(ITestOutputHelper output) {
            _output = output;
            _rnd = new();
        }

        [Fact(DisplayName = "Lengths")]
        public void TestLengths() {
            for (var i = 0; i < Globals.Iterations; i++) {
                var keySize = _rnd.Next(Globals.MinKeyLength, Globals.MaxKeyLength);
                using var algorithm = new PohligHellman(keySize);
                Assert.Equal(keySize, algorithm.KeySize);

                var prms = algorithm.ExportParameters();
                var P = new BigInteger(prms.P);
                var E = new BigInteger(prms.E);
                var D = new BigInteger(prms.D);

                Assert.Equal(keySize, P.BitCount());
                Assert.True(1 < E && E <= P - 2);
                Assert.True(1 < D && D <= P - 2);
                Assert.Equal(BigInteger.One, (E * D) % (P - 1));

                algorithm.Dispose();
            }
        }
    }
}
