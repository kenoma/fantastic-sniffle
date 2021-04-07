using System;
using System.IO;
using System.Linq;
using System.Text;
using eNFN.eANFIS.Impl;
using eNFN.FIS;
using eNFN.FIS.MembershipFunctions;
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
            const double iters = 10000;
            var error = 0.0;
            for (var i = 0; i < iters; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = fis.Inference(new[] {px}, lx);
                error += Math.Abs(lx - predicted);
            }

            error = 0;
            var ppx = lx;
            for (var i = 0; i < 80; i++)
            {
                //px = lx;
                lx = r * lx * (1 - lx);
                ppx = fis.Inference(new[] {ppx});
                error += Math.Abs(lx - ppx);
            }

            Assert.Less(error / 80, 1e-2);
        }

        [Test, Explicit]
        public void InternalStructure()
        {
            var sbStruct = new StringBuilder();
            var sbError = new StringBuilder();
            var sbPred = new StringBuilder();
            sbStruct.AppendLine("r;c_x");
            sbError.AppendLine("r;err;count");
            sbPred.AppendLine("r;x;predicted_x");
            
            var fis = Create();
            
            for (var r = 1.1; r < 3.88; r += 1e-3)
            {
                var px = 0.1;
                var lx = r * px * (1 - px);
                for (var i = 0; i < 100000; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    fis.Inference(new[] {px}, lx);
                }

                for (var i = 0; i < 80; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    var pred = fis.Inference(new[] {px});
                    sbPred.AppendLine($"{r};{lx};{pred}");
                }

                sbError.AppendLine($"{r};{fis.GeneralError};{fis.TermLayers[0].Cores.Length}");
                foreach (var core in fis.TermLayers[0].Cores.Where(z => double.IsFinite(z.X)))
                {
                    sbStruct.AppendLine($"{r};{core.X}\r\n");
                }
            }

            File.WriteAllText("logistic_map_1d_fis.csv",
                sbStruct.ToString());
            File.WriteAllText("logistic_map_1d_fis_error.csv",
                sbError.ToString());
            File.WriteAllText("logistic_map_1d_fis_pred.csv",
                sbPred.ToString());
        }

        private InferenceLayer<LinearMembershipFunction> Create()
        {
            var lrate = 1e-3;
            var smoothingAvarageRate = 1e-3;

            var ruleset = new FirstLevelRuleset(2e-1);

            var termLayer = new TermLayer<LinearMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 200, competitionLooseLimit: 100); //TermCore.Create(0), TermCore.Create(1)
            var fis = new InferenceLayer<LinearMembershipFunction>(new[] {termLayer}, ruleset, smoothingAvarageRate);
            return fis;
        }
    }
}