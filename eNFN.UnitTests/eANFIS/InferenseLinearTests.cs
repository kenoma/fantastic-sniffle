using System.Collections.Generic;
using eNFN.eANFIS;
using eNFN.eANFIS.Impl;
using Moq;
using NUnit.Framework;

namespace eNFN.UnitTests.eANFIS
{
    public class InferenseLinearTests
    {
        private IRuleset _ruleset;
        private ITermLayer _termLayer;

        [SetUp]
        public void Setup()
        {
            _ruleset = new FirstLevelRuleset();
            _termLayer = new LinearTermLayer();
        }

        [Test]
        public void LearningStep_1DCase()
        {
            var fis = CreateFIS();

            var result = fis.LearningStep(5, 1.0);

            Assert.AreEqual(5, result);
        }

        private Inference CreateFIS()
        {
            return new(_ruleset, new[] {_termLayer});
        }
    }
}