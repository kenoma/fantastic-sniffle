using NUnit.Framework;

namespace eNFN.UnitTests
{
    public class UpdateQTests
    {
        [Test]
        public void ContextAdaptationStep_AdjustMax_10()
        {
            var neuron = new NeoFuzzyNeuron(0, 1, 2, alpha: 1);

            neuron.UpdateQ(0, 0.5, 2);

            Assert.AreEqual(-1, neuron.Q[0]);
        }
    }
}