using System;

namespace eNFN.FIS
{
    public class LinearMembershipFunction : IMembershipFunction
    {
        public double Mu(double x, params double[] param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));
            if (param.Length != 2)
                throw new ArgumentException("Incorrect paramset.", nameof(param));

            var a = param[0];
            var b = param[1];

            if (double.IsInfinity(b))
                return 1.0;
            
            if (double.IsInfinity(a))
                return 0.0;

            if ((x > a && x > b) || (x < a && x < b))
                return 0.0;

            return a < b ? (b - x) / (b - a) : (1.0 - (x - a) / (b - a));
        }
    }
}