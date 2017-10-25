using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace RangePermute.Tests
{
    [TestClass]
    public class RangePermuteTests
    {
        [TestMethod]
        public void TestSmallRanges()
        {
            for (int rangeMax = 0; rangeMax < 1000; rangeMax++)
            {
                var idealRange = Enumerable.Range(0, rangeMax).Aggregate(new HashSet<ulong>(), (hs, i) => { hs.Add((ulong)i); return hs; });
                var algoRange = RangeEnumerable.Range((ulong)rangeMax).Aggregate(new HashSet<ulong>(), (hs, i) => { hs.Add((ulong)i); return hs; });

                idealRange.SymmetricExceptWith(algoRange);
                Assert.AreEqual(idealRange.Count, 0);
            }
        }

        [TestMethod]
        public void TestLargeRange()
        {
            var testRange = 100000;
            var seen = new BitArray(testRange);
            foreach (var idx in RangeEnumerable.Range((ulong)testRange))
            {
                Assert.IsFalse(seen[(int)idx]);
                seen[(int)idx] = true;
            }
        }

    }
}
