using System;
using eNFN.FIS;
using NUnit.Framework;

namespace eNFN.UnitTests.FIS
{
    public class LinearMembershipFunctionTests
    {
        [TestCase(-1, ExpectedResult = 0)]
        [TestCase(2, ExpectedResult = 0)]
        [TestCase(double.NegativeInfinity, ExpectedResult = 0)]
        [TestCase(double.PositiveInfinity, ExpectedResult = 0)]
        public double MuTestBounds(double input)
        {
            var memf = Create();

            var retval = memf.Mu(input, 0, 1);

            return Math.Round(retval, 2);
        }


        [TestCase(double.NegativeInfinity, 0, ExpectedResult = 0)]
        [TestCase(double.PositiveInfinity, 0, ExpectedResult = 0)]
        [TestCase(0, double.NegativeInfinity, ExpectedResult = 1)]
        [TestCase(0, double.PositiveInfinity, ExpectedResult = 1)]
        [TestCase(double.PositiveInfinity, double.NegativeInfinity, ExpectedResult = 1)]
        [TestCase(double.PositiveInfinity, double.PositiveInfinity, ExpectedResult = 1)]
        public double MuTestInfiniteBound(double lowerbound, double upperbound)
        {
            var memf = Create();

            var retval = memf.Mu(0, lowerbound, upperbound);

            return Math.Round(retval, 2);
        }

        [TestCase(0, ExpectedResult = 1)]
        [TestCase(1, ExpectedResult = 0)]
        [TestCase(0.5, ExpectedResult = 0.5)]
        [TestCase(0.2, ExpectedResult = 0.8)]
        [TestCase(0.8, ExpectedResult = 0.2)]
        public double MuTestAB(double input)
        {
            var memf = Create();

            var retval = memf.Mu(input, 0, 1);

            return Math.Round(retval, 2);
        }

        [TestCase(1, ExpectedResult = 1)]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(0.5, ExpectedResult = 0.5)]
        [TestCase(0.2, ExpectedResult = 0.2)]
        [TestCase(0.8, ExpectedResult = 0.8)]
        public double MuTestBA(double input)
        {
            var memf = Create();

            var retval = memf.Mu(input, 1, 0);

            return Math.Round(retval, 2);
        }

        private static LinearMembershipFunction Create() => new();
    }
}