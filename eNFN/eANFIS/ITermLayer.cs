namespace eNFN.eANFIS
{
    public interface ITermLayer
    {
        LayerFiring GetMuValues(double x);
        
        void Adapt(int termId, double error, double inferenceForRule);
    }
}