namespace eNFN.eANFIS
{
    public class FiringData
    {
        public static FiringData CreateInstance(int termId, double firingLevel) => new(termId, firingLevel);

        private FiringData(int termId, double firingLevel)
        {
            TermId = termId;
            FiringLevel = firingLevel;
        }

        public int TermId { get; }

        public double FiringLevel { get; }
    }
}