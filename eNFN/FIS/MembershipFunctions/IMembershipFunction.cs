namespace eNFN.FIS
{
    public interface IMembershipFunction
    {
        public double Mu(double x, params double[] param);
    }
}