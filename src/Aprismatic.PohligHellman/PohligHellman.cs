using System;
using System.Numerics;
using System.Security.Cryptography;

namespace Aprismatic.PohligHellman
{
    public class PohligHellman : IDisposable
    {
        private readonly PohligHellmanKeyStruct _keyStruct;
        public readonly int KeySize;

        public BigInteger MaxEncryptableValue => _keyStruct.PminusOne;
        public BigInteger P => _keyStruct.P;
        public int CiphertextLength => _keyStruct.CiphertextLength;

        private readonly RandomNumberGenerator _rng;

        // TODO: Constructor should probably optionally accept an RNG
        public PohligHellman(int keySize) {
            _rng = RandomNumberGenerator.Create();
            KeySize = keySize;
            var p = BigInteger.One.GenPseudoPrime(KeySize, 16, _rng);
            _keyStruct = CreateKeyPair(p);
        }

        public PohligHellman(BigInteger p) {
            _rng = RandomNumberGenerator.Create();
            KeySize = p.BitCount();
            _keyStruct = CreateKeyPair(p);
        }

        // TODO: Consolidate constructors in one method
        public PohligHellman(PohligHellmanParameters prms) {
            _rng = RandomNumberGenerator.Create();
            _keyStruct = new PohligHellmanKeyStruct(
                new BigInteger(prms.P),
                new BigInteger(prms.E),
                new BigInteger(prms.D)
            );
            KeySize = _keyStruct.PBitCount;
        }

        public PohligHellman(string xml) : this(PohligHellmanParameters.FromXml(xml)) { }

        // TODO: This method should probably move to KeyStruct
        private PohligHellmanKeyStruct CreateKeyPair(BigInteger P) {
            BigInteger E, D;
            var PminusOne = P - BigInteger.One;

            do {
                E = BigInteger.Zero.GenRandomBits(2, PminusOne, _rng);
            } while (BigInteger.GreatestCommonDivisor(E, PminusOne) != BigInteger.One);

            D = E.ModInverse(PminusOne);

            return new PohligHellmanKeyStruct(P, E, D);
        }

        public byte[] EncryptData(BigInteger message) {
            if (message < 0 || message > MaxEncryptableValue)
                throw new ArgumentException($"Value should be 0 <= m <= {MaxEncryptableValue} under the current encryption key", nameof(message));

            var ctbs = _keyStruct.CiphertextLength;
            var array = new byte[ctbs];
            var arsp = array.AsSpan();

            var res = BigInteger.ModPow(message, _keyStruct.E, _keyStruct.P);
            res.TryWriteBytes(arsp, out _);

            return array;
        }

        public BigInteger DecryptData(byte[] data) {
            var a = new BigInteger(data);
            var m = BigInteger.ModPow(a, _keyStruct.D, _keyStruct.P);
            return m;
        }

        public PohligHellmanParameters ExportParameters() => _keyStruct.ExportParameters();

        public string ToXmlString() {
            var prms = ExportParameters();
            return prms.ToXml();
        }

        public void Dispose() => _rng.Dispose();
    }
}
