using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace eNFN.UnitTests
{
    public class LogisticsMapTest
    {
        private NeoFuzzyNetwork _enfn;

        [Test]
        public void LogisticsMapCase()
        {
            _enfn = new NeoFuzzyNetwork(1, xmin: 0, xmax: 1, m: 2, maxM: 200, alpha: 1e-2, beta: 1e-2, ageLimit: 100);
            var px = 0.1;
            const double r = 3.580999999999727; //3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            const double iters = 10000;
            for (var i = 0; i < iters; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = _enfn.InferenceWithLearning(new[] {px}, lx);
                error += Math.Abs(lx - predicted);
            }

            Assert.Less(error / iters, 1.0);
        }
        
        [Test, Explicit]
        public void LogisticsMapCaseDataset()
        {
            var sbStruct = new StringBuilder();
            var sbError = new StringBuilder();
            var sbPred = new StringBuilder();
            sbStruct.AppendLine("r;c_x");
            sbError.AppendLine("r;err;count");
            sbPred.AppendLine("r;x;predicted_x");
            
            var fis = new NeoFuzzyNetwork(1, xmin: 0, xmax: 1, m: 2, maxM: 200, alpha: 1e-2, beta: 1e-2, ageLimit: 100);
            
            for (var r = 1.1; r < 3.88; r += 1e-3)
            {
                var px = 0.1;
                var lx = r * px * (1 - px);
                for (var i = 0; i < 100000; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    fis.InferenceWithLearning(new[] {px}, lx);
                }

                for (var i = 0; i < 80; i++)
                {
                    px = lx;
                    lx = r * lx * (1 - lx);
                    var pred = fis.Inference(new[] {px});
                    sbPred.AppendLine($"{r};{lx};{pred}");
                }

                sbError.AppendLine($"{r};{Math.Abs(fis.GeneralError)};{fis._layers[0].Size}");
                foreach (var core in fis._layers[0].B)
                {
                    sbStruct.AppendLine($"{r};{core}\r\n");
                }
            }

            File.WriteAllText("silva_logistic_map_1d_fis.csv",
                sbStruct.ToString());
            File.WriteAllText("silva_logistic_map_1d_fis_error.csv",
                sbError.ToString());
            File.WriteAllText("silva_logistic_map_1d_fis_pred.csv",
                sbPred.ToString());
        }
        
        [Test]
        public void LogisticsMap2dCase()
        {
            _enfn = new NeoFuzzyNetwork(2, xmin: 0, xmax: 1, m: 2, maxM: 100, alpha: 1e-2, beta: 1e-2, ageLimit: 10000);
            var px = 0.1;
            const double r = 3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            for (int i = 0; i < 100000; i++)
            {
                var inp = new double[] { px, lx };
                px = lx;
                lx = r * lx * (1 - lx);
                
                var expected = lx;
                var predicted = _enfn.InferenceWithLearning(inp, expected);
                error += Math.Abs(expected - predicted);
            }

            Assert.Less(error / 100000, 1.0);
        }
    }
}