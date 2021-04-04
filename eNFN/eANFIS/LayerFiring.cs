using System;
using System.Linq;
using Microsoft.VisualBasic;

namespace eNFN.eANFIS
{
    public class LayerFiring
    {
        private readonly FiringData[] _firings;

        public LayerFiring(params FiringData[] firingData)
        {
            _firings = firingData ?? throw new ArgumentNullException(nameof(firingData));
        }

        public int TermsCount => _firings.Length;

        public FiringData this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _firings[index];
            }
        }

        public override string ToString() =>
            $"Terms: {string.Join(", ", _firings.Select(z => z.TermId))}; Sum: {_firings.Sum(z => z.FiringLevel)}";
    }
}