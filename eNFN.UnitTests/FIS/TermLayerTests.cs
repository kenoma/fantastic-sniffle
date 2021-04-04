using System;
using System.Linq;
using eNFN.FIS;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class TermLayerTests
    {
        private readonly TermCore _zero = TermCore.Create(0);
        private readonly TermCore _half = TermCore.Create(0.5);
        private readonly TermCore _one = TermCore.Create(1);

        [Test]
        public void GetActivation_CheckIfOutputCorrect_0()
        {
            var layer = Create();

            var result = layer.GetActivation(0).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(_zero.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(1.0, result.FirstOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_0minus()
        {
            var layer = Create();

            var result = layer.GetActivation(-0.1).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(_zero.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(1.0, result.FirstOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_1()
        {
            var layer = Create();

            var result = layer.GetActivation(1).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(_one.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(1.0, result.FirstOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_1plus()
        {
            var layer = Create();

            var result = layer.GetActivation(1.1).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(_one.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(1.0, result.FirstOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_half()
        {
            var layer = Create();

            var result = layer.GetActivation(0.5).ToArray();

            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(_half.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(1.0, result.FirstOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_ZeroAndHalf()
        {
            var layer = Create();

            var result = layer.GetActivation(0.25).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_zero.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(_half.Id, result.LastOrDefault().TermId);
            Assert.AreEqual(0.5, result.FirstOrDefault().FiringLevel);
            Assert.AreEqual(0.5, result.LastOrDefault().FiringLevel);
        }

        [Test]
        public void GetActivation_CheckIfOutputCorrect_HalfAndOne()
        {
            var layer = Create();

            var result = layer.GetActivation(0.75).ToArray();

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(_half.Id, result.FirstOrDefault().TermId);
            Assert.AreEqual(_one.Id, result.LastOrDefault().TermId);
            Assert.AreEqual(0.5, result.FirstOrDefault().FiringLevel);
            Assert.AreEqual(0.5, result.LastOrDefault().FiringLevel);
        }

        [TestCase(-0.2, 1.2)]
        [TestCase(0.2, 1.2)]
        [TestCase(0.2, 0.8)]
        [TestCase(-0.5, 0.8)]
        public void BackpropError_CoresLearnedCorrectly(double initialZero, double initialOne)
        {
            var zero = TermCore.Create(initialZero);
            var one = TermCore.Create(initialOne);
            var layers = new TermLayer(new LinearMembershipFunction(), new[]
            {
                zero, one
            }, learningRate: 1e-1);

            for (var i = 0; i < 100; i++)
            {
                layers.BackpropError(0, -zero.X);
                layers.BackpropError(1, 1.0 - one.X);
            }

            Assert.AreEqual(0, zero.X, 1e-2);
            Assert.AreEqual(1, one.X, 1e-2);
        }
        
        [Test]
        public void BackpropError_AccumulatedErrorComputedCorrectly()
        {
            var zero = TermCore.Create(0);
            var one = TermCore.Create(1);
            var layers = new TermLayer(new LinearMembershipFunction(), new[]
            {
                zero, one
            }, learningRate: 1e-1, smoothingAverageRate: 1e-1);

            for (var i = 0; i < 100; i++)
            {
                layers.BackpropError(0, 5);
                layers.BackpropError(1, -3);
            }

            Assert.AreEqual(5, zero.AccumulatedError, 1e-2);
            Assert.AreEqual(-3, one.AccumulatedError, 1e-2);
        }

        [TestCase(0.3)]
        [TestCase(0.5)]
        [TestCase(0.8)]
        public void CreationStep_CorrectlyCreatedMidleTerm(double input)
        {
            var zero = TermCore.Create(0);
            var one = TermCore.Create(1);
            zero.AccumulatedError = 1;
            one.AccumulatedError = 1;
            var layers = new TermLayer(new LinearMembershipFunction(), new[]
            {
                zero, one
            });

            layers.CreationStep(input, 0, 0);

            Assert.AreEqual(5, layers.Cores.Length);
            Assert.AreEqual(0.5, layers.Cores[2].X);
        }
        
        [TestCase(2.0)]
        [TestCase(-1.0)]
        public void CreationStep_CorrectlyOutsideTerm(double input)
        {
            var zero = TermCore.Create(0);
            var one = TermCore.Create(1);
            zero.AccumulatedError = 1;
            one.AccumulatedError = 1;
            var layers = new TermLayer(new LinearMembershipFunction(), new[]
            {
                zero, one
            });

            layers.CreationStep(input, 0, 0);

            Assert.AreEqual(5, layers.Cores.Length);
            Assert.IsTrue(layers.Cores.Any(z => Math.Abs(z.X - input) < 1e-5));
        }
        
        [TestCase(2.0)]
        [TestCase(-1.0)]
        public void CreationStep_InitialInfinities(double input)
        {
            var layers = new TermLayer(new LinearMembershipFunction());

            layers.CreationStep(input, 0, 0);

            Assert.AreEqual(3, layers.Cores.Length);
            Assert.IsTrue(layers.Cores.Any(z => Math.Abs(z.X - input) < 1e-5));
        }

        private TermLayer Create() => new(new LinearMembershipFunction(), new[] {_zero, _one, _half});
    }
}