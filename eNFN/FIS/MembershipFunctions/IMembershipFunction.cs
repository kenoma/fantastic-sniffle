namespace eNFN.FIS.MembershipFunctions
{
    public interface IMembershipFunction
    {
        public double Mu(double x, params double[] param);
    }
}