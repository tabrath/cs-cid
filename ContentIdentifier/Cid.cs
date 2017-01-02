using System;
using System.Linq;
using System.Text;
using BinaryEncoding;
using Multiformats.Base;
using Multiformats.Codec;
using Multiformats.Hash;

namespace ContentIdentifier
{
    public class Cid
    {
        private readonly ulong _version;

        public MulticodecCode Type { get; }
        public Multihash Hash { get; }
        public string KeyString => Encoding.UTF8.GetString(ToBytes());
        public Prefix Prefix => new Prefix(_version, Type, Hash.Code, Hash.Length);

        protected Cid(ulong version, MulticodecCode codec, Multihash hash)
        {
            _version = version;
            Type = codec;
            Hash = hash;
        }

        public Cid(Multihash hash)
            : this(0, MulticodecCode.DagProtobuf, hash)
        {
        }

        public Cid(MulticodecCode codec, Multihash hash)
            : this(1, codec, hash)
        {
        }

        public static Cid Parse(string s) => Decode(s.Contains("/ipfs/") ? s.Split(new[] {"/ipfs/"}, StringSplitOptions.RemoveEmptyEntries)[1] : s);
        public static Cid Parse(byte[] b) => Cast(b);
        public static Cid Parse(Multihash mh) => new Cid(mh);
        public static Cid Parse(Cid cid) => new Cid(cid._version, cid.Type, cid.Hash);

        public static Cid Decode(string v)
        {
            if (v.Length < 2)
                throw new ArgumentException("cid too short", nameof(v));

            if (v.Length == 46 && v.Substring(0, 2) == "Qm")
                return new Cid(Multihash.Parse(v));

            return Cast(Multibase.Decode(v));
        }

        public static Cid Cast(byte[] data)
        {
            if (data.Length == 34 && data[0] == 18 && data[1] == 32)
                return new Cid(Multihash.Cast(data));
            
            ulong vers;
            var n = Binary.Varint.Read(data, 0, out vers);
            if (vers != 0 && vers != 1)
                throw new Exception($"invalid cid version number: {vers}");

            ulong codec;
            var cn = Binary.Varint.Read(data, n, out codec);

            var rest = new byte[data.Length - (n + cn)];
            Buffer.BlockCopy(data, n + cn, rest, 0, rest.Length);

            var hash = Multihash.Cast(rest);

            return new Cid(vers, (MulticodecCode)codec, hash);
        }

        public override string ToString()
        {
            switch (_version)
            {
                case 0:
                    return Hash.ToString(Multibase.Base58);
                case 1:
                    return Multibase.Encode(Multibase.Base58, ToBytes());
                default:
                    throw new Exception("unsupported version");
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Cid;
            if (other == null)
                return false;

            return Type == other.Type &&
                   _version == other._version &&
                   Hash.Equals(other.Hash);
        }

        public override int GetHashCode() => (int)_version ^ (int) Type ^ Hash.GetHashCode();

        public byte[] ToBytes()
        {
            switch (_version)
            {
                case 0:
                    return Hash;
                case 1:
                    return Binary.Varint.GetBytes(_version)
                        .Concat(Binary.Varint.GetBytes((ulong) Type))
                        .Concat((byte[]) Hash)
                        .ToArray();
                default:
                    throw new Exception("unsupported version");
            }
        }

        public byte[] MarshalJson() => Encoding.UTF8.GetBytes($"\"{this}\"");

        public static Cid UnmarshalJson(byte[] b)
        {
            if (b.Length < 2)
                throw new ArgumentException("invalid cid json blob", nameof(b));

            return Decode(Encoding.UTF8.GetString(b, 1, b.Length - 1));
        }

        public static implicit operator Cid(byte[] b) => Parse(b);
        public static implicit operator Cid(string s) => Parse(s);
        public static implicit operator Cid(Multihash mh) => Parse(mh);
        public static implicit operator byte[](Cid cid) => cid.ToBytes();
        public static implicit operator string(Cid cid) => cid.ToString();
        public static implicit operator Multihash(Cid cid) => cid.Hash;
    }
}
