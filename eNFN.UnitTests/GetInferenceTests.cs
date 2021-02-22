using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace eNFN.UnitTests
{
    [TestFixture]
    public class NeoFuzzyNeuronTests
    {
        private NeoFuzzyNeuron _neuron;
        
        [SetUp]
        public void Setup()
        {
            _neuron = new NeoFuzzyNeuron(new[] {0.0, 1.0, 2.0, 3.0}, new[] {1.0, 0, 0, 2.0});
        }

        [TestCase(-1, ExpectedResult = 1)]
        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 0)]
        [TestCase(2, ExpectedResult = 0)]
        [TestCase(3, ExpectedResult = 2)]
        [TestCase(4, ExpectedResult = 2)]
        [TestCase(0.5, ExpectedResult = 0.5)]
        [TestCase(1.5, ExpectedResult = 0.0)]
        [TestCase(2.5, ExpectedResult = 1)]
        [TestCase(0.2, ExpectedResult = 0.8)]
        public double GetInference_Inference(double inputX)
        {
            return _neuron.GetInference(inputX).Inference;
        }
        
        [TestCase(-1, ExpectedResult = 0)]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(1, ExpectedResult = 1)]
        [TestCase(2, ExpectedResult = 2)]
        [TestCase(3, ExpectedResult = 3)]
        [TestCase(4, ExpectedResult = 3)]
        [TestCase(0.3, ExpectedResult = 0)]
        [TestCase(0.7, ExpectedResult = 1)]
        [TestCase(2.4, ExpectedResult = 2)]
        [TestCase(2.6, ExpectedResult = 3)]
        public double GetInference_ActiveMembership(double inputX)
        {
            return _neuron.GetInference(inputX).MostActiveFunc;
        }
        
        [TestCase(-1, ExpectedResult = 1)]
        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 3)]
        [TestCase(3, ExpectedResult = 2)]
        [TestCase(4, ExpectedResult = 2)]
        [TestCase(0.3, ExpectedResult = 1)]
        [TestCase(0.7, ExpectedResult = 0)]
        [TestCase(2.4, ExpectedResult = 3)]
        [TestCase(2.6, ExpectedResult = 2)]
        public double GetInference_LessActiveMembership(double inputX)
        {
            return _neuron.GetInference(inputX).LessActiveFunc;
        }
        
        [TestCase(-1, ExpectedResult = 1.0)]
        [TestCase(0, ExpectedResult = 1.0)]
        [TestCase(1, ExpectedResult = 1.0)]
        [TestCase(2, ExpectedResult = 1.0)]
        [TestCase(3, ExpectedResult = 1.0)]
        [TestCase(4, ExpectedResult = 1.0)]
        [TestCase(0.3, ExpectedResult = 0.7)]
        [TestCase(0.5, ExpectedResult = 0.5)]
        [TestCase(0.7, ExpectedResult = 0.7)]
        public double GetInference_Mu(double inputX)
        {
            return _neuron.GetInference(inputX).MuValue;
        }
    }
}