using System;
using System.Collections.Generic;
using System.Linq;

namespace eNFN.eANFIS.Impl
{
    public class FirstLevelRuleset : IRuleset
    {
        private readonly double _learningRate;

        private readonly Dictionary<IReadOnlyList<Guid>, double> _rules =
            new(new GuidArrayEqualityComparer());

        public FirstLevelRuleset(double learningRate = 5e-2)
        {
            _learningRate = learningRate;
        }

        public double GetInferenceForRule(IReadOnlyList<double> inputX, IReadOnlyList<Guid> ruleCode, double? expected)
        {
            if (ruleCode == null)
                throw new ArgumentNullException(nameof(ruleCode));

            if (_rules.TryGetValue(ruleCode, out var retval))
            {
                return retval;
            }

            _rules.Add(ruleCode, expected ?? 0);
            return expected ?? 0;
        }

        public void BackpropError(IReadOnlyList<double> _, IReadOnlyList<Guid> termIds, double error, double firingLevel)
        {
            _rules[termIds] = _rules[termIds] + _learningRate * error * firingLevel;
        }

        public void EliminateRules(int layer, Guid eliminatingTerm)
        {
            foreach (var key in _rules.Keys.ToArray())
            {
                if (key[layer] == eliminatingTerm)
                {
                    _rules.Remove(key);
                }
            }
        }
    }
}