using System;
using System.Linq;
using BinaryEncoding;
using Multiformats.Codec;
using Multiformats.Hash;

namespace ContentIdentifier
{
    public class Prefix
    {
        public ulong Version { get; }
        public MulticodecCode Codec { get; }
        public HashType MultihashType { get; }
        public int MultihashLength { get; }

        internal Prefix(ulong version, MulticodecCode codec, HashType mhType, int mhLength)
        {
            Version = version;
            Codec = codec;
            MultihashType = mhType;
            MultihashLength = mhLength;
        }

        public Prefix(byte[] buf)
        {
            ulong vers;
            var offset = Binary.Varint.Read(buf, 0, out vers);
            ulong codec;
            offset += Binary.Varint.Read(buf, offset, out codec);
            ulong mhtype;
            offset += Binary.Varint.Read(buf, offset, out mhtype);
            ulong mhlength;
            offset += Binary.Varint.Read(buf, offset, out mhlength);

            Version = vers;
            Codec = (MulticodecCode) codec;
            MultihashType = (HashType) mhtype;
            MultihashLength = (int) mhlength;
        }

        public Cid Sum(byte[] data)
        {
            var hash = Multihash.Sum(MultihashType, data, MultihashLength);

            switch (Version)
            {
                case 0:
                    return new Cid(hash);
                case 1:
                    return new Cid(Codec, hash);
                default:
                    throw new Exception("invalid cid version");
            }
        }

        public byte[] ToBytes() => Binary.Varint.GetBytes(Version)
            .Concat(Binary.Varint.GetBytes((ulong) Codec))
            .Concat(Binary.Varint.GetBytes((ulong) MultihashType))
            .Concat(Binary.Varint.GetBytes((ulong) MultihashLength))
            .ToArray();

        public override bool Equals(object obj)
        {
            var other = obj as Prefix;
            if (other == null)
                return false;

            return Version == other.Version &&
                   Codec == other.Codec &&
                   MultihashType == other.MultihashType &&
                   MultihashLength == other.MultihashLength;
        }
    }
}