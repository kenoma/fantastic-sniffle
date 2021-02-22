using NUnit.Framework;

namespace eNFN.UnitTests
{
    [TestFixture]
    public class ModalValuesAdaptationTests
    {
        [TestCase(0)]
        [TestCase(2)]
        public void ModalValuesAdaptation_DoNotTouchBorder(int activeFunc)
        {
            var neuron = new NeoFuzzyNeuron(0, 2, 3, beta: 1);

            neuron.ModalValuesAdaptation(double.MaxValue, activeFunc);

            Assert.AreEqual(0, neuron.XMin);
            Assert.AreEqual(2, neuron.XMax);
        }
        
        [Test]
        public void ModalValuesAdaptation_AdjustActiveFunction()
        {
            var neuron = new NeoFuzzyNeuron(0, 2, 3, beta: 1);

            neuron.ModalValuesAdaptation(0.1, 1);

            Assert.AreEqual(0.1, neuron.B[1], 1e-5);
        }
    }
}