using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiformats.Base;
using Multiformats.Hash;
using Multiformats.Hash.Algorithms;
using NUnit.Framework;

namespace ContentIdentifier.Tests
{
    [TestFixture]
    public class CidTests
    {
        [Test]
        public void TestBasicMarshaling()
        {
            var h = Multihash.Sum<SHA3_512>(Encoding.UTF8.GetBytes("TEST"), 4);
            var cid = new Cid(CidCodec.DagCBOR, h);

            var data = cid.ToBytes();
            var output = Cid.Cast(data);
            Assert.That(output, Is.EqualTo(cid));

            var s = cid.ToString();
            output = Cid.Decode(s);
            Assert.That(output, Is.EqualTo(cid));
        }

        [Test]
        public void TestEmptyString()
        {
            Assert.Throws<ArgumentException>(() => Cid.Decode(""));
        }

        [Test]
        public void TestV0Handling()
        {
            var old = "QmdfTbBqBPQ7VNxZEYEj14VmRuZBkqFbiwReogJgS1zR1n";
            var cid = Cid.Decode(old);

            Assert.That(cid.Prefix.Version, Is.EqualTo(0));
            Assert.That(cid.Hash.ToString(Multibase.Base58), Is.EqualTo(old));
            Assert.That(cid.ToString(), Is.EqualTo(old));
        }

        [Test]
        public void TestPrefixRoundtrip()
        {
            var data = Encoding.UTF8.GetBytes("this is some test content");
            var hash = Multihash.Sum<SHA2_256>(data);
            var c = new Cid(CidCodec.DagCBOR, hash);
            var pref = c.Prefix;
            var c2 = pref.Sum(data);

            Assert.That(c2, Is.EqualTo(c));

            var pb = pref.ToBytes();
            var pref2 = new Prefix(pb);

            Assert.That(pref2, Is.EqualTo(pref));
        }

        [Test]
        public void Test16BytesVarint()
        {
            var data = Encoding.UTF8.GetBytes("this is some test content");
            var hash = Multihash.Sum<SHA2_256>(data);
            var c = new Cid((CidCodec)(1UL << 63), hash);

            Assert.DoesNotThrow(() => c.ToBytes());
        }
    }
}
