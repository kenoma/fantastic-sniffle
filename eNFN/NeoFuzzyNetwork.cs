using System;
using System.Linq;

namespace eNFN
{
    public class NeoFuzzyNetwork
    {
        private readonly int _inputDimention;
        private readonly double _beta;
        public readonly NeoFuzzyNeuron[] _layers;
        private double _globalError;
        private double _globalStd;
        
        public double GeneralError => _globalError;

        public NeoFuzzyNetwork(int inputDimention, double xmin = 0, double xmax = 1, int m = 2, int maxM = 100, double alpha=1e-3, double beta = 1e-2, int ageLimit = 100)
        {
            if (inputDimention <= 0)
                throw new ArgumentOutOfRangeException(nameof(inputDimention));

            _inputDimention = inputDimention;
            _beta = beta;
            _layers = Enumerable.Range(0, inputDimention)
                .Select(z => new NeoFuzzyNeuron(xmin, xmax, m, maxM, alpha, beta, ageLimit)).ToArray();
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

        public double Inference(double[] x)
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

            return sum;
        }
    }
}