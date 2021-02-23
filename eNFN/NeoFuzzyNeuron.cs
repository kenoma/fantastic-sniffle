using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("eNFN.UnitTests")]

namespace eNFN
{
    public class NeoFuzzyNeuron
    {
        private readonly int _maxM;
        private readonly double _alpha;
        private readonly double _beta;
        private readonly int _ageLimit;

        private readonly List<Term> _pins;
        private int _epoch;

        public double XMin => _pins[0].CoreX;
        public double XMax => _pins[^1].CoreX;

        public double[] B => _pins.Select(z => z.CoreX).ToArray();
        
        public double[] Q => _pins.Select(z => z.Q).ToArray();

        public int Size => _pins.Count;

        public int Epoch => _epoch;

        public NeoFuzzyNeuron(double xmin = 0, double xmax = 1, int m = 2, int maxM = 100, double alpha=1e-3, double beta = 1e-2, int ageLimit = 100)
        {
            if (beta <= 0)
                throw new ArgumentOutOfRangeException(nameof(beta));
            if (m <= 1) 
                throw new ArgumentOutOfRangeException(nameof(m));
            if (xmax <= xmin) 
                throw new ArgumentOutOfRangeException(nameof(xmax));
            if (alpha <= 0)
                throw new ArgumentOutOfRangeException(nameof(alpha));
            if (maxM <= m) 
                throw new ArgumentOutOfRangeException(nameof(maxM));
            if (ageLimit <= 0) 
                throw new ArgumentOutOfRangeException(nameof(ageLimit));

            _pins = new List<Term>();
            for (var i = 0; i < m; i++)
            {
                _pins.Add(new Term
                {
                    CoreX = xmin + i * (xmax - xmin) / (m - 1)
                });
            }

            _maxM = maxM;
            _alpha = alpha;
            _beta = beta;
            _ageLimit = ageLimit;
        }

        internal NeoFuzzyNeuron(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
        {
            if (x.Count != y.Count)
                throw new InvalidDataException();

            _pins = x
                .Zip(y, (a, b) => new Term {CoreX = a, Q = b})
                .OrderBy(z => z.CoreX)
                .ToList();
        }

        public (double Inference, int MostActiveFunc, int LessActiveFunc, double MuValue) GetInference(double input)
        {
            var target = -1;
            for (var i = 0; i < _pins.Count - 1; i++)
            {
                if (input >= _pins[i].CoreX && input < _pins[i + 1].CoreX)
                {
                    target = i;
                    break;
                }
            }

            if (target == -1)
            {
                var most = input <= _pins[0].CoreX ? 0 : _pins.Count - 1;
                var less = input <= _pins[0].CoreX ? 1 : _pins.Count - 2;
                return (input < _pins[0].CoreX ? _pins[0].Q : _pins[^1].Q, most, less, 1.0);
            }

            var rate = (_pins[target + 1].CoreX - input) / (_pins[target + 1].CoreX - _pins[target].CoreX);

            var vmost = rate < 0.5 ? (target + 1) : target;
            var vless = rate < 0.5 ? target : (target + 1);

            return ((1.0 - rate) * _pins[target + 1].Q + rate * _pins[target].Q,
                vmost, vless, vmost == target ? rate : (1.0 - rate));
        }

        public void LearningStep(double input, double error, double globalErrorMean, double globalErrorStd)
        {
            ContextAdaptationStep(input);
            var (_, mostActiveFunc, lessActiveFunc, mu) = GetInference(input);
            ModalValuesAdaptation(input, mostActiveFunc);
            UpdateQ(mostActiveFunc, mu, error);
            UpdateQ(lessActiveFunc, 1.0 - mu, error);
            MembershipFunctionsCreationStep(mostActiveFunc, lessActiveFunc, mu, error, globalErrorMean,
                globalErrorStd);
            MembershipFunctionsElimination(mostActiveFunc, lessActiveFunc);
        }

        internal void MembershipFunctionsElimination(int mostActiveFunc, int lessActiveFunc)
        {
            _epoch++;
            _pins[mostActiveFunc].EpochUpdated = _epoch;
            _pins[lessActiveFunc].EpochUpdated = _epoch;
            var elder = _pins
                .OrderByDescending(z => _epoch - z.EpochUpdated)
                .First();
            
            if (_pins.Count <= 2 || _epoch - elder.EpochUpdated < _ageLimit) 
                return;
            
            var index = _pins.IndexOf(elder);
            if (index == 0)
            {
                _pins[1].CoreX = elder.CoreX;

            }
            else if (index == _pins.Count - 1)
            {
                _pins[^2].CoreX = elder.CoreX;
            }

            _pins.Remove(elder);
        }

        internal void MembershipFunctionsCreationStep(int mostActiveFunc, 
            int lessActive, 
            double mu, 
            double error, 
            double globalErrorMean, 
            double globalErrorStd)
        {
            var amemb = _pins[mostActiveFunc];
            
            amemb.MeanError -= _beta * (amemb.MeanError - error);
            if (amemb.MeanError > globalErrorMean + globalErrorStd)
            {
                var tau = (XMax - XMin) / _maxM;
                var dist = 0.0;
                if (mostActiveFunc != 0 && mostActiveFunc != _pins.Count - 1)
                {
                    dist = (_pins[mostActiveFunc + 1].CoreX - _pins[mostActiveFunc - 1].CoreX) / 3.0;
                    if (dist > tau)
                    {
                        _pins.Remove(amemb);
                        _pins.Add(new Term
                        {
                            CoreX = _pins[mostActiveFunc - 1].CoreX + dist,
                            Q = amemb.Q,
                            MeanError = 0
                        });
                        _pins.Add(new Term
                        {
                            CoreX = _pins[mostActiveFunc - 1].CoreX + 2.0 * dist,
                            Q = amemb.Q,
                            MeanError = 0
                        });
                    }
                }else if (mostActiveFunc == 0)
                {
                    dist = (_pins[1].CoreX - _pins[0].CoreX) / 2;
                    if (dist > tau)
                    {
                        _pins.Add(new Term
                        {
                            Q = amemb.Q,
                            CoreX = XMin + dist,
                            MeanError = 0
                        });
                    }
                }
                else
                {
                    dist = (_pins[^1].CoreX - _pins[^2].CoreX) / 2;
                    if (dist > tau)
                    {
                        _pins.Add(new Term
                        {
                            Q = amemb.Q,
                            CoreX = XMax - dist,
                            MeanError = 0
                        });
                    }
                }
            }

            _pins.Sort((x, y) => x.CoreX.CompareTo(y.CoreX));
        }


        internal void UpdateQ(int mostActiveFunc, double mu, double error)
        {
            _pins[mostActiveFunc].Q -= _alpha * error * mu;
        }

        internal void ModalValuesAdaptation(double input, int mostActiveFunc)
        {
            if (mostActiveFunc == 0 || mostActiveFunc == _pins.Count - 1)
                return;
            
            _pins[mostActiveFunc].CoreX += _beta * (input - _pins[mostActiveFunc].CoreX);
        }

        internal void ContextAdaptationStep(double input)
        {
            if (input < _pins[0].CoreX)
                _pins[0].CoreX = input;
            
            if (input > _pins[^1].CoreX)
                _pins[^1].CoreX = input;
        }

        private class Term
        {
            public double CoreX { get; set; }

            public double Q { get; set; }

            public double MeanError { get; set; }

            public int EpochUpdated { get; set; }

            public override string ToString() =>
                $"x: {CoreX:0.000} q: {Q:0.000} Error: {MeanError:g1} Epoch: {EpochUpdated}";
        }
    }
}