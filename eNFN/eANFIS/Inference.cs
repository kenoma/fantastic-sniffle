using System;
using System.Collections.Generic;
using System.Linq;

namespace eNFN.eANFIS
{
    public class Inference
    {
        private readonly IReadOnlyList<ITermLayer> _termLayers;
        private readonly IRuleset _rulesets;

        public Inference(IRuleset rulesets, IReadOnlyList<ITermLayer> termLayers)
        {
            _rulesets = rulesets ?? throw new ArgumentNullException(nameof(rulesets));
            _termLayers = termLayers ?? throw new ArgumentNullException(nameof(termLayers));
        }

        /// <summary>
        ///     Compute inference in ANFIS-like manner without adaptations
        /// </summary>
        /// <param name="x">Input value to map x->y</param>
        /// <returns>y</returns>
        public double Compute(params double[] x)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            
            if (x.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(x));

            if (x.Length != _termLayers.Count)
                throw new ArgumentException("Input value length cannot differs from fis structure.", nameof(x));

            var mu = new LayerFiring[x.Length];

            for (var i = 0; i < x.Length; i++)
            {
                mu[i] = _termLayers[i].GetMuValues(x[i]);
            }

            return RecursiveComputeOutput(x, mu, new int[x.Length], 0);
        }

        public double LearningStep(double expected, params double[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            
            if (input.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(input));

            if (input.Length != _termLayers.Count)
                throw new ArgumentException("Input value length cannot differs from fis structure.", nameof(input));

            var mu = new LayerFiring[input.Length];

            for (var i = 0; i < input.Length; i++)
            {
                mu[i] = _termLayers[i].GetMuValues(input[i]);
            }

            var learningData = new List<(double Firing, int[] RuleCode, double InferenceForRule)>();
            var retval = RecursiveComputeOutput(input, mu, new int[input.Length], 0, learningData);

            var error = expected - retval;
            foreach (var (firing, ruleCode, inferenceForRule) in learningData)
            {
                //_rulesets.Adapt(ruleCode, error, firing);
                for (var layer = 0; layer < _termLayers.Count; layer++)
                {
                    _termLayers[layer].Adapt(ruleCode[layer], error, inferenceForRule);
                }
            }

            return retval;
        }

        private double RecursiveComputeOutput(IReadOnlyList<double> input, IReadOnlyList<LayerFiring> mu, IReadOnlyList<int> muPath,
            int depth, ICollection<(double Firing, int[] RuleCode, double InferenceForRule)> learningData = null)
        {
            if (depth == input.Count)
            {
                var firing = 1.0;
                var ruleCode = new int[input.Count];
                for (var m = 0; m < input.Count; m++)
                {
                    var firingData = mu[m][muPath[m]];
                    firing *= firingData.FiringLevel;
                    ruleCode[m] = firingData.TermId;
                }

                // var inferenceForRule = _rulesets.GetInferenceForRule(input, ruleCode);
                //
                // learningData?.Add((firing, ruleCode, inferenceForRule));

                return firing;//* inferenceForRule;
            }

            var retval = 0.0;
            for (var i = 0; i < mu[depth].TermsCount; i++)
            {
                var pathCopy = muPath.ToArray();
                pathCopy[depth] = i;
                retval += RecursiveComputeOutput(input, mu, pathCopy, depth + 1, learningData);
            }

            return retval;
        }
    }
}