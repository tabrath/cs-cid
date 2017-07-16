using System;
using System.Text;
using Multiformats.Base;
using Multiformats.Codec;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;
using Xunit;

namespace IPLD.ContentIdentifier.Tests
{
    public class CidTests
    {
        [Fact]
        public void TestBasicMarshaling()
        {
            var h = Multihash.Sum<SHA3_512>(Encoding.UTF8.GetBytes("TEST"), 4);
            var cid = new Cid(MulticodecCode.MerkleDAGCBOR, h);

            var data = cid.ToBytes();
            var output = Cid.Cast(data);
            Assert.Equal(cid, output);

            var s = cid.ToString();
            output = Cid.Decode(s);
            Assert.Equal(cid, output);
        }

        [Fact]
        public void TestEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Cid.Decode(""));
        }

        [Fact]
        public void TestV0Handling()
        {
            var old = "QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zR1n";
            var cid = Cid.Decode(old);

            Assert.Equal((ulong)0, cid.Prefix.Version);
            Assert.Equal(old, cid.Hash.ToString(Multibase.Base58));
            Assert.Equal(old, cid.ToString());
        }

        [Fact]
        public void TestPrefixRoundtrip()
        {
            var data = Encoding.UTF8.GetBytes("this is some test content");
            var hash = Multihash.Sum<SHA2_256>(data);
            var c = new Cid(MulticodecCode.MerkleDAGCBOR, hash);
            var pref = c.Prefix;
            var c2 = pref.Sum(data);

            Assert.Equal(c, c2);

            var pb = pref.ToBytes();
            var pref2 = new Prefix(pb);

            Assert.Equal(pref, pref2);
        }

        [Fact]
        public void Test16BytesVarint()
        {
            var data = Encoding.UTF8.GetBytes("this is some test content");
            var hash = Multihash.Sum<SHA2_256>(data);
            var c = new Cid((MulticodecCode)(1UL << 63), hash);

            c.ToBytes();
        }
    }
}
