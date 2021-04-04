using System;
using eNFN.eANFIS.Impl;
using eNFN.FIS;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class InferenceLayerTests
    {
        [Test]
        public void LogisticsMapCase()
        {
            var fis = Create();
            var px = 0.1;
            const double r = 3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            for (var i = 0; i < 100000; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = fis.Inference(new[] {px}, lx);
                error += Math.Abs(lx - predicted);
            }

            Assert.Less(error / 100000, 1.0);
        }

        private InferenceLayer Create()
        {
            var lrate = 1e-2;
            var smoothingAvarageRate = 1e-2;

            var ruleset = new FirstLevelRuleset(lrate);
            var memf = new LinearMembershipFunction();
            var termLayer = new TermLayer(memf, learningRate: lrate, smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 100, ageLimit: 1000);
            var fis = new InferenceLayer(new[] {termLayer}, ruleset, smoothingAvarageRate);
            return fis;
        }
    }
}