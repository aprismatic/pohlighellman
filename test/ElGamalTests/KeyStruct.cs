using System;
using System.Numerics;
using Aprismatic;
using Aprismatic.PohligHellman;
using Xunit;
using Xunit.Abstractions;

namespace ElGamalTests
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
                using var algorithm = new PohligHellman(_rnd.Next(8, 4096));
                var prms = algorithm.ExportParameters();

                var P = new BigInteger(prms.P);
                Assert.Equal(algorithm.KeySize, P.BitCount());

                algorithm.Dispose();
            }
        }
    }
}
