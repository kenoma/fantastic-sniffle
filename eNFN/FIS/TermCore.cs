using System;
using System.Reflection.Metadata.Ecma335;

namespace eNFN.FIS
{
    public class TermCore
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public double X { get; set; }

        public double AccumulatedError { get; set; }
        
        public ulong EpochActivated { get; set; }

        public static TermCore Create(double x) => new TermCore {X = x};

        public static TermCore Create(double x, ulong epochActivated) =>
            new TermCore {X = x, EpochActivated = epochActivated};
    }
}