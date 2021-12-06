using System.Numerics;

namespace Aprismatic.PohligHellman
{
    public struct PohligHellmanKeyStruct
    {
        // PUBLIC KEY
        public readonly BigInteger P; // Modulus

        // PRIVATE KEY
        public readonly BigInteger E;
        public readonly BigInteger D;

        // CONSTRUCTOR
        public PohligHellmanKeyStruct(BigInteger p, BigInteger e, BigInteger d) {
            P = p;
            PBitCount = p.BitCount();
            PminusOne = P - BigInteger.One;

            E = e;
            D = d;

            CiphertextLength = ((PBitCount + 7) << 3) + 1; // We add 1 because last bit of a BigInteger is reserved to store its sign.
        }

        // HELPER VALUES
        // These values are derived from the pub/priv key and precomputed for faster processing
        public readonly int PBitCount;
        public readonly BigInteger PminusOne;
        public readonly int CiphertextLength;

        public PohligHellmanParameters ExportParameters() {
            // set the public values of the parameters
            var prms = new PohligHellmanParameters {
                P = P.ToByteArray(),
                E = E.ToByteArray(),
                D = D.ToByteArray()
            };

            return prms;
        }
    }
}
