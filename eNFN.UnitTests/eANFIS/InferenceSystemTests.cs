// using System;
// using System.Collections.Generic;
// using eNFN.eANFIS;
// using Moq;
// using NUnit.Framework;
//
// namespace eNFN.UnitTests.eANFIS
// {
//     public class InferenceSystemTests
//     {
//
//         private Mock<IRuleset> _ruleset;
//         private Mock<ITermLayer> _termLayer;
//         private MockRepository _moqRepository;
//
//         [SetUp]
//         public void Setup()
//         {
//             _moqRepository = new MockRepository(MockBehavior.Default);
//             _ruleset = _moqRepository.Create<IRuleset>();
//             _termLayer = _moqRepository.Create<ITermLayer>();
//         }
//
//         [Test]
//         public void Compute_1DCase()
//         {
//             _termLayer.Setup(z => z.GetMuValues(It.IsAny<double>()))
//                 .Returns(new LayerFiring(FiringData.CreateInstance(1, 0.5), FiringData.CreateInstance(2, 0.5)));
//
//             // _ruleset.Setup(
//             //         z => z.GetInferenceForRule(It.IsAny<IReadOnlyList<double>>(),
//             //             It.Is<IReadOnlyList<int>>(ints => ints[0] == 1)))
//             //     .Returns(0);
//             // _ruleset.Setup(
//             //         z => z.GetInferenceForRule(It.IsAny<IReadOnlyList<double>>(),
//             //             It.Is<IReadOnlyList<int>>(ints => ints[0] == 2)))
//             //     .Returns(1);
//             
//             var fis = CreateFIS();
//
//             var result = fis.Compute(1.0);
//
//             Assert.AreEqual(0.5, result);
//         }
//         
//         [Test]
//         public void LearningStep_1DCase()
//         {
//             _termLayer.Setup(z => z.GetMuValues(It.IsAny<double>()))
//                 .Returns(new LayerFiring(FiringData.CreateInstance(1, 0.5), FiringData.CreateInstance(2, 0.5)));
//
//             _ruleset.Setup(
//                     z => z.GetInferenceForRule(It.IsAny<IReadOnlyList<double>>(),
//                         It.Is<IReadOnlyList<int>>(ints => ints[0] == 1)))
//                 .Returns(0);
//             _ruleset.Setup(
//                     z => z.GetInferenceForRule(It.IsAny<IReadOnlyList<double>>(),
//                         It.Is<IReadOnlyList<int>>(ints => ints[0] == 2)))
//                 .Returns(1);
//             
//             var fis = CreateFIS();
//
//             var result = fis.LearningStep(5, 1.0);
//
//             Assert.AreEqual(0.5, result);
//         }
//
//         private Inference CreateFIS()
//         {
//             return new(_ruleset.Object, new[] {_termLayer.Object});
//         }
//     }
// }