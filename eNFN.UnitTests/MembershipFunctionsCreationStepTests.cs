using System.Net.Sockets;
using NUnit.Framework;

namespace eNFN.UnitTests
{
    [TestFixture]
    public class MembershipFunctionsCreationStepTests
    {
        [Test]
        public void MembershipFunctionsCreationStep_AddNewTermInternal_ListSorted()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10);

            neuron.MembershipFunctionsCreationStep(1, 0, 0.8, 50, 0, 0);

            Assert.AreEqual(4, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
            Assert.AreEqual(2.0 / 3.0, neuron.B[1]);
            Assert.AreEqual(4.0 / 3.0, neuron.B[2]);
        }
        
        [Test]
        public void MembershipFunctionsCreationStep_AddNewTermInternal_Left()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10);

            neuron.MembershipFunctionsCreationStep(0, 0, 0.8, 50, 0, 0);

            Assert.AreEqual(4, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
            Assert.AreEqual(0, neuron.B[0]);
            Assert.AreEqual(1.0 / 2.0, neuron.B[1]);
        }
        
        [Test]
        public void MembershipFunctionsCreationStep_AddNewTermInternal_Right()
        {
            var neuron = new NeoFuzzyNeuron(xmin: 0, xmax: 2, m: 3, maxM: 10);

            neuron.MembershipFunctionsCreationStep(2, 0, 0.8, 50, 0, 0);

            Assert.AreEqual(4, neuron.Size);
            CollectionAssert.IsOrdered(neuron.B);
            Assert.AreEqual(2, neuron.B[^1]);
            Assert.AreEqual(3.0 / 2.0, neuron.B[^2]);
        }
    }
}