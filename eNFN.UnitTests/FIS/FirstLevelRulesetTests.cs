using System;
using eNFN.eANFIS;
using eNFN.eANFIS.Impl;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class FirstLevelRulesetTests
    {
        private IRuleset _ruleset;
        
        [SetUp]
        public void Setup()
        {
            _ruleset = new FirstLevelRuleset(learningRate: 1e-1);
        }
        
        [TestCase(10)]
        [TestCase(-10)]
        [TestCase(0)]
        public void BackpropError_LearnedTargetValue(double targetValue)
        {
            var key = new[] {Guid.NewGuid()};

            var currentValue = _ruleset.GetInferenceForRule(Array.Empty<double>(), key);
            for (var i = 0; i < 100; i++)
            {
                _ruleset.BackpropError(Array.Empty<double>(), key, targetValue - currentValue, 1.0);
                currentValue = _ruleset.GetInferenceForRule(Array.Empty<double>(), key);
            }

            Assert.AreEqual(targetValue, currentValue, 1e-2);
        }
    }
}