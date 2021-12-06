﻿using System;
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
            _keyStruct = CreateKeyPair();
        }

        public PohligHellman(BigInteger P) {
            _rng = RandomNumberGenerator.Create();
            KeySize = P.BitCount();
            _keyStruct = CreateKeyPair(P);
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
        private PohligHellmanKeyStruct CreateKeyPair() {
            BigInteger P;

            do {
                P = BigInteger.One.GenPseudoPrime(KeySize, 16, _rng);
            } while (P.BitCount() != KeySize);

            return CreateKeyPair(P);
        }

        private PohligHellmanKeyStruct CreateKeyPair(BigInteger P) {
            BigInteger E, D;
            var PminusTwo = P - 2;

            do {
                E = BigInteger.Zero.GenPseudoPrime(KeySize, 16, _rng);
            } while (!(E <= 1 || E >= PminusTwo));

            D = E.ModInverse(P - BigInteger.One);

            return new PohligHellmanKeyStruct(P, E, D);
        }

        public byte[] EncryptData(BigInteger message) {
            if (message < 0 || message > MaxEncryptableValue)
                throw new ArgumentException($"Value should be 0 <= m <= {MaxEncryptableValue}", nameof(message));

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
