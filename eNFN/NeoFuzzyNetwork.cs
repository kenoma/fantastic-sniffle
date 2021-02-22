﻿using System;
using System.Linq;

namespace eNFN
{
    public class NeoFuzzyNetwork
    {
        private readonly int _inputDimention;
        private readonly double _beta;
        private NeoFuzzyNeuron[] _layers;
        private double _globalError;
        private double _globalStd;

        public NeoFuzzyNetwork(int inputDimention, double beta = 1e-2)
        {
            if (inputDimention <= 0)
                throw new ArgumentOutOfRangeException(nameof(inputDimention));

            _inputDimention = inputDimention;
            _beta = beta;
            _layers = Enumerable.Range(0, inputDimention).Select(z => new NeoFuzzyNeuron(beta: beta)).ToArray();
        }

        public double InferenceWithLearning(double[] x, double expected)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (x.Length != _inputDimention)
                throw new ArgumentException(nameof(x.Length));

            var sum = 0.0;
            for (var i = 0; i < _inputDimention; i++)
            {
                var (inference, _, _, _) = _layers[i].GetInference(x[i]);
                sum += inference;
            }

            var error = sum - expected;
            _globalError -= _beta * (_globalError - error);
            _globalStd = (1 - _beta) * (_globalStd + _beta * Math.Pow(_globalError - error, 2));

            for (var i = 0; i < _inputDimention; i++)
            {
                _layers[i].LearningStep(x[i], error, _globalError, _globalStd);
            }

            return sum;
        }
    }
}