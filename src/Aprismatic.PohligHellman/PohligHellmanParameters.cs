using System;
using System.Text;
using System.Xml.Linq;

namespace Aprismatic.PohligHellman
{
    [Serializable]
    public struct PohligHellmanParameters
    {
        // public portion
        public byte[] P;

        // private portion
        public byte[] E;
        public byte[] D;

        public static PohligHellmanParameters FromXml(string xml) {
            var res = new PohligHellmanParameters();

            var kv = XDocument.Parse(xml).Element("PohligHellmanKeyValue");

            var kvelP = kv.Element("P");
            if (kvelP == null)
                throw new ArgumentException("Provided XML does not have a public key value `P`");
            res.P = Convert.FromBase64String(kvelP.Value);

            var kvelE = kv.Element("E");
            if (kvelE == null)
                throw new ArgumentException("Provided XML does not have a public key value `E`");
            res.E = Convert.FromBase64String(kvelE.Value);

            var kvelD = kv.Element("D");
            if (kvelD == null)
                throw new ArgumentException("Provided XML does not have a public key value `D`");
            res.D = Convert.FromBase64String(kvelD.Value);

            return res;
        }

        public string ToXml() {
            var sb = new StringBuilder();

            sb.Append("<PohligHellmanKeyValue>");

            sb.Append("<P>" + Convert.ToBase64String(P) + "</P>");
            sb.Append("<E>" + Convert.ToBase64String(E) + "</E>");
            sb.Append("<D>" + Convert.ToBase64String(D) + "</D>");

            sb.Append("</PohligHellmanKeyValue>");

            return sb.ToString();
        }
    }
}
