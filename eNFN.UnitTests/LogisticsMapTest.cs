using System;
using System.IO;
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
            _enfn = new NeoFuzzyNetwork(1, xmin: 0, xmax: 1, m: 2, maxM: 100, alpha: 1e-2, beta: 1e-2, ageLimit: 10000);
            var px = 0.1;
            const double r = 3.8;
            var lx = r * px * (1 - px);

            var error = 0.0;
            for (var i = 0; i < 100000; i++)
            {
                px = lx;
                lx = r * lx * (1 - lx);
                var predicted = _enfn.InferenceWithLearning(new[] {px}, lx);
                error += Math.Abs(lx - predicted);
            }

            Assert.Less(error / 100000, 1.0);
        }
        
        [Test]
        public void LogisticsMapCaseDataset()
        {
            _enfn = new NeoFuzzyNetwork(1, xmin: 0, xmax: 1, m: 2, maxM: 100, alpha: 1e-2, beta: 1e-2, ageLimit: 10000);
            var px = 0.1;
            var error = 0.0;
            
            using(var fs = File.OpenWrite("F:\\trash\\ocr\\dataset_struct.csv"))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine("r;c_x");

                for (var r = 1.0; r < 3.8; r+=1e-2)
                {

                    var lx = r * px * (1 - px);

                    for (var i = 0; i < 100000; i++)
                    {
                        px = lx;
                        lx = r * lx * (1 - lx);
                        var predicted = _enfn.InferenceWithLearning(new[] {px}, lx);
                        error += Math.Abs(lx - predicted);
                    }

                    foreach (var core in _enfn._layers[0].B)
                    {
                        sw.WriteLine($"{r};{core}");
                    }
                }
            }

            Assert.Less(error / 100000, 1.0);
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