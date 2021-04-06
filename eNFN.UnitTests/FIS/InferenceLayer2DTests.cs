using System;
using eNFN.eANFIS.Impl;
using eNFN.FIS;
using eNFN.FIS.MembershipFunctions;
using eNFN.FIS.Terms;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class InferenceLayer2DTests
    {
        [Test,Explicit]
        public void LogisticsMapCase()
        {
            var fis = Create();
            var px = 0.1;
            const double r = 3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            for (var i = 0; i < 100000; i++)
            {
                var tmp = px;
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = fis.Inference(new[] {tmp, px}, lx);
                error += Math.Abs(lx - predicted);
            }

            Assert.Less(error / 100000, 1e-3);
        }

        private InferenceLayer<LinearMembershipFunction> Create()
        {
            var lrate = 2e-1;
            var smoothingAvarageRate = 1e-1;

            var ruleset = new FirstLevelRuleset(lrate);
            
            var termLayer1 = new TermLayer<LinearMembershipFunction>(learningRate: lrate, smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 1000, competitionLooseLimit: 10000);
            var termLayer2 = new TermLayer<LinearMembershipFunction>(learningRate: lrate, smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 1000, competitionLooseLimit: 10000);
            var fis = new InferenceLayer<LinearMembershipFunction>(new[] {termLayer1, termLayer2}, ruleset,
                smoothingAvarageRate);
            return fis;
        }
    }
}