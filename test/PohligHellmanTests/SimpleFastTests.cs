using System;
using System.Numerics;
using System.Security.Cryptography;
using Aprismatic.PohligHellman;
using Xunit;
using Xunit.Abstractions;

namespace PohligHellmanTests
{
    public class SimpleFastTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        private readonly Random _rnd = new();
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
        
        public SimpleFastTests(ITestOutputHelper output) {
            _output = output;
        }

        public void Dispose() {
            _rng.Dispose();
        }

        [Fact(DisplayName = "Specific cases")]
        public void TestSpecificCases() {
            {
                var algorithm = new PohligHellman(256);

                var a = new BigInteger(2048);
                var aBytes = algorithm.EncryptData(a);
                var decA = algorithm.DecryptData(aBytes);
                Assert.Equal(a, decA);

                algorithm.Dispose();
            }

            {
                var algorithm = new PohligHellman(256);

                var a = new BigInteger(138);
                var aBytes = algorithm.EncryptData(a);
                var decA = algorithm.DecryptData(aBytes);

                Assert.Equal(a, decA);

                algorithm.Dispose();
            }
        }

        /*[Fact(DisplayName = "Simple negatives")]
        public void TestNegativeCases() {
            { // Simple negative cases
                using var algorithm = new PohligHellman(256);

                //Negative Number En/Decryption
                var a = new BigInteger(-94660895);
                var aEnc = algorithm.EncryptData(a);
                var aDec = algorithm.DecryptData(aEnc);
                Assert.Equal(a, aDec);

                var b = new BigInteger(45651255);
                var bEnc = algorithm.EncryptData(b);
                var bDec = algorithm.DecryptData(bEnc);
                Assert.Equal(b, bDec);
            }
        }*/
    }
}
