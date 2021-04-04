using System;
using System.Collections.Generic;
using System.Linq;
using eNFN.eANFIS;
using eNFN.FIS.Terms;

namespace eNFN.FIS
{
    public class InferenceLayer<T> where T : IMembershipFunction, new()
    {
        private readonly TermLayer<T>[] _termLayers;
        private readonly IRuleset _ruleset;
        private readonly double _smoothingAverageRate;
        private double _generalErrorAverage = 0.0;
        private double _generalErrorStd = 0.0;

        public TermLayer<T>[] TermLayers => _termLayers.ToArray();
        public double GeneralError => _generalErrorAverage;

        public InferenceLayer(TermLayer<T>[] termLayers, IRuleset ruleset, double smoothingAverageRate=1e-2)
        {
            _termLayers = termLayers ?? throw new ArgumentNullException(nameof(termLayers));
            _ruleset = ruleset ?? throw new ArgumentNullException(nameof(ruleset));
            _smoothingAverageRate = smoothingAverageRate;
        }

        public double Inference(IReadOnlyList<double> inputX, double expected)
        {
            if (inputX?.Count != _termLayers.Length)
                throw new ArgumentNullException(nameof(inputX));

            var learningData =
                new List<(IReadOnlyList<Guid> TermsCombo, double Firing)>();
            var retval = RecursiveComputeOutput(inputX, new Guid[inputX.Count], new double[inputX.Count], 1.0, 0,
                learningData);

            var error = expected - retval;
            foreach (var (termsCombo, generalFiring) in learningData)
            {
                _ruleset.BackpropError(inputX, termsCombo, error, generalFiring);
            }
            
            _generalErrorStd = (1 - _smoothingAverageRate) * (_generalErrorStd +
                                                             _smoothingAverageRate * 
                                                             (_generalErrorAverage - Math.Abs(error)) *
                                                             (_generalErrorAverage - Math.Abs(error)));
            _generalErrorAverage -= _smoothingAverageRate * (_generalErrorAverage - Math.Abs(error));
            
            for (var layer = 0; layer < _termLayers.Length; layer++)
            {
                _termLayers[layer].BackpropError(inputX[layer], Math.Abs(error));
                _termLayers[layer].CreationStep(inputX[layer], _generalErrorAverage, _generalErrorStd);
            }
            
            for (var layer = 0; layer < _termLayers.Length; layer++)
            {
                if (_termLayers[layer].TryEliminateTerm(out var eliminatingTerm))
                {
                    _ruleset.EliminateRules(layer, eliminatingTerm);
                }
            }
            
            return retval;
        }

        private double RecursiveComputeOutput(
            IReadOnlyList<double> input,
            IReadOnlyList<Guid> termsCombo,
            IReadOnlyList<double> firings,
            double generalFiring,
            int depth,
            ICollection<(IReadOnlyList<Guid> TermsCombo, double GeneralFiring)> learningData)
        {
            if (depth == input.Count)
            {
                var inferenceForRule = _ruleset.GetInferenceForRule(input, termsCombo);
                learningData.Add((termsCombo, generalFiring));

                return generalFiring * inferenceForRule;
            }

            var layerActivation = _termLayers[depth].GetActivation(input[depth]);

            var sum = 0.0;
            foreach (var (firingLevel, termId) in layerActivation)
            {
                if (firingLevel < 1e-20)
                {
                    continue;
                }

                var tmpTermCombo = termsCombo.ToArray();
                var tmpFirings = firings.ToArray();
                tmpTermCombo[depth] = termId;
                tmpFirings[depth] = firingLevel;
                sum += RecursiveComputeOutput(input, tmpTermCombo, tmpFirings, generalFiring * firingLevel, depth + 1,
                    learningData);
            }

            return sum;
        }
    }
}