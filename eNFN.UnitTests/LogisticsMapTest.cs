using System;
using NUnit.Framework;

namespace eNFN.UnitTests
{
    public class LogisticsMapTest
    {
        private NeoFuzzyNetwork _enfn;

        [Test]
        public void LogisticsMapCase()
        {
            _enfn = new NeoFuzzyNetwork(1, 1e-1);
            var px = 0.1;
            const double r = 3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            for (var i = 0; i < 100000; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = _enfn.InferenceWithLearning(new[] {px}, lx);
                error = Math.Abs(lx - predicted);
            }

            Assert.Less(error / 1000, 1.0);
        }
    }
}