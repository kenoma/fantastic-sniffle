using System;
using System.IO;
using System.Linq;
using eNFN.eANFIS.Impl;
using eNFN.FIS;
using eNFN.FIS.Terms;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class InferenceLayer1DTests
    {
        [Test, Explicit]
        public void LogisticsMapCase()
        {
            var fis = Create();
            var px = 0.1;
            const double r = 3.580999999999727; //3.8;
            var lx = r * px * (1 - px);
            const double iters = 100000;
            var error = 0.0;
            for (var i = 0; i < iters; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = fis.Inference(new[] {px}, lx);
                error += Math.Abs(lx - predicted);
            }

            Assert.Less(error / iters, 1e-2);
        }

        [Test, Explicit]
        public void InternalStructure()
        {
            using var fsPred = File.OpenWrite("logistic_map_1d_fis_pred.csv");
            using var swPred = new StreamWriter(fsPred);

            using var fsError = File.OpenWrite("logistic_map_1d_fis_error.csv");
            using var swError = new StreamWriter(fsError);

            using var fs = File.OpenWrite("logistic_map_1d_fis.csv");
            using var sw = new StreamWriter(fs);

            sw.WriteLine("r;c_x");
            swError.WriteLine("r;err");
            swPred.WriteLine("r;x;predicted_x");
            var fis = Create();
            
            for (var r = 1.1; r < 3.88; r += 3e-3)
            {
                var px = 0.1;
                var lx = r * px * (1 - px);
                for (var i = 0; i < 1000000; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    fis.Inference(new[] {px}, lx);
                }

                for (var i = 0; i < 100; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    var pred = fis.Inference(new[] {px}, lx);
                    swPred.WriteLine($"{r};{lx};{pred}");
                }

                swError.WriteLine($"{r};{fis.GeneralError}");
                foreach (var core in fis.TermLayers[0].Cores.Where(z => double.IsFinite(z.X)))
                {
                    sw.WriteLine($"{r};{core.X}");
                }
            }
        }

        private InferenceLayer<LinearMembershipFunction> Create()
        {
            var lrate = 2e-3;
            var smoothingAvarageRate = 1e-2;

            var ruleset = new FirstLevelRuleset(lrate);

            var termLayer = new TermLayer<LinearMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 100, competitionLooseLimit: 100); //TermCore.Create(0), TermCore.Create(1)
            var fis = new InferenceLayer<LinearMembershipFunction>(new[] {termLayer}, ruleset, smoothingAvarageRate);
            return fis;
        }
    }
}