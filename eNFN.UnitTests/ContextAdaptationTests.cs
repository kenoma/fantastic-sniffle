using NUnit.Framework;

namespace eNFN.UnitTests
{
    [TestFixture]
    public class ContextAdaptationTests
    {
        [Test]
        public void ContextAdaptationStep_AdjustMin_MinusOne()
        {
            var neuron = new NeoFuzzyNeuron(0, 1, 2);

            neuron.ContextAdaptationStep(-1);

            Assert.AreEqual(-1, neuron.XMin);
        }
        
        [Test]
        public void ContextAdaptationStep_AdjustMax_10()
        {
            var neuron = new NeoFuzzyNeuron(0, 1, 2);

            neuron.ContextAdaptationStep(10);

            Assert.AreEqual(10, neuron.XMax);
        }
    }
}