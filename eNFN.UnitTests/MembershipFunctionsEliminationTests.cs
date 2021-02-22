using NUnit.Framework;

namespace eNFN.UnitTests
{
    public class MembershipFunctionsEliminationTests
    {
        [Test]
        public void MembershipFunctionsElimination_RemoveCenter()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10, ageLimit: 1);

            neuron.MembershipFunctionsElimination(0, 2);

            Assert.AreEqual(2, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
        }
        
        [Test]
        public void MembershipFunctionsElimination_RemoveLeft()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10, ageLimit: 1);

            neuron.MembershipFunctionsElimination(1, 2);

            Assert.AreEqual(2, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
        }
        
        [Test]
        public void MembershipFunctionsElimination_RemoveRight()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10, ageLimit: 1);

            neuron.MembershipFunctionsElimination(0, 1);

            Assert.AreEqual(2, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
        }
    }
}