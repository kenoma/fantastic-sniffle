using System;
using System.Collections.Generic;
using System.Linq;

namespace eNFN.eANFIS.Impl
{
    public class LinearTermLayer : ITermLayer
    {
        private int _counter = 0;
        private class Term
        {
            public int Id { get; set; }
            public double Center { get; set; }

            public FiringData ToFiringData(double firingLevel) => FiringData.CreateInstance(Id, firingLevel);
        }

        private readonly List<Term> _terms = new List<Term>();

        public LinearTermLayer()
        {
            
        }

        public LayerFiring GetMuValues(double x)
        {
            if (_terms.Count < 2)
            {
                var initialValue = new Term
                {
                    Center = x,
                    Id = _counter++
                };
                _terms.Add(initialValue);
                _terms.Sort((x, y) => x.Center.CompareTo(y.Center));
                return new LayerFiring(initialValue.ToFiringData(1));
            }

            var target = -1;
            for (var i = 0; i < _terms.Count - 1; i++)
            {
                if (x >= _terms[i].Center && x < _terms[i + 1].Center)
                {
                    target = i;
                    break;
                }
            }

            if (target == -1)
            {
                return new LayerFiring(FiringData.CreateInstance(_terms[x < _terms[0].Center ? 0 : ^1].Id, 1.0));
            }

            var a = _terms[target].Center;
            var b = _terms[target + 1].Center;

            return new LayerFiring(
                _terms[target].ToFiringData((b - x) / (b - a)),
                _terms[target + 1].ToFiringData((x - a) / (a - b)));
        }

        public void Adapt(int termId, double error, double inferenceForRule)
        {
            //throw new System.NotImplementedException();
        }
    }
}