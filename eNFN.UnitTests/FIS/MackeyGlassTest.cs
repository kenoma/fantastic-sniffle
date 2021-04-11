using System;
using System.Globalization;
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
    public class MackeyGlassTest
    {
        
        [Test, Explicit]
        public void RumMackeyGlass()
        {
            var dataset = File.ReadLines("FIS\\mackey_glass.csv")
                .Select(z=>double.Parse(z, NumberStyles.Any, CultureInfo.InvariantCulture))
                .ToArray();

            var dsep = Convert.ToInt32(dataset.Length * 0.9);
            var testing = dataset.Skip(dsep).ToArray();
            dataset = dataset.Take(dsep).ToArray();
            
            var sbStruct = new StringBuilder();
            var sbError = new StringBuilder();
            var sbPred = new StringBuilder();
            sbStruct.AppendLine("iter;c_x");
            sbError.AppendLine("iter;err;count");
            sbPred.AppendLine("iter;x;predicted_x");
            
            var fis = Create();
            
            for (var j = 18; j<dataset.Length-6; j++)
            {
                var input = new []{dataset[j-18],dataset[j-12],dataset[j-6],dataset[j]};
                var expected = dataset[j + 6];
                
                fis.Inference(input, expected);
                
                sbError.AppendLine($"{j};{fis.GeneralError};{fis.TermLayers[0].Cores.Length}");
                foreach (var core in fis.TermLayers[0].Cores.Where(z => double.IsFinite(z.X)))
                {
                    sbStruct.AppendLine($"{j};{core.X}\r\n");
                }
            }

            var error = 0.0;
            for (var j = 18; j < testing.Length - 6; j++)
            {
                var input = new[] {testing[j - 18], testing[j - 12], testing[j - 6], testing[j]};
                var expected = testing[j + 6];

                var pred = fis.Inference(input);
                sbPred.AppendLine($"{j};{expected};{pred}");
                error += Math.Abs(pred - expected);
            }
            
            File.WriteAllText("mackey_glass_1d_fis.csv",
                sbStruct.ToString());
            File.WriteAllText("mackey_glass_1d_fis_error.csv",
                sbError.ToString());
            File.WriteAllText("mackey_glass_1d_fis_pred.csv",
                sbPred.ToString());
            error /= testing.Length - 24;
            Assert.Less(error, 1e-2);
        }

        private InferenceLayer<PiMembershipFunction> Create()
        {
            var lrate = 1e-3;
            var smoothingAvarageRate = 1e-3;

            var ruleset = new FirstLevelRuleset(1e-1);

            var termLayer1 = new TermLayer<PiMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 30, competitionLooseLimit: 500);
            var termLayer2 = new TermLayer<PiMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 30, competitionLooseLimit: 500);
            var termLayer3 = new TermLayer<PiMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 30, competitionLooseLimit: 500);
            var termLayer4 = new TermLayer<PiMembershipFunction>(learningRate: lrate,
                smoothingAverageRate: smoothingAvarageRate,
                termsLimit: 30, competitionLooseLimit: 500);
            var fis = new InferenceLayer<PiMembershipFunction>(new[] {termLayer1, termLayer2, termLayer3, termLayer4},
                ruleset, smoothingAvarageRate);
            return fis;
        }
    }
}