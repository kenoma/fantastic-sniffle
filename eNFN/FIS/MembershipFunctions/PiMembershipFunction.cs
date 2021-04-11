using System;

namespace eNFN.FIS.MembershipFunctions
{
    public class PiMembershipFunction : IMembershipFunction
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

            if (a < b)
            {
                if (x <= (a + b) / 2)
                    return 1 - 2 * Math.Pow((x - a) / (b - a), 2);
                if (x <= b)
                    return 2 * Math.Pow((x - b) / (b - a), 2);
            }
            else
            {
                if (x <= (a + b) / 2)
                    return 2 * Math.Pow((x - b) / (b - a), 2);
                if (x <= a)
                    return 1 - 2 * Math.Pow((x - a) / (b - a), 2);
            }

            return 0.0;
        }
    }
}