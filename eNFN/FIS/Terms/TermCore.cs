using System;
using System.Reflection.Metadata.Ecma335;

namespace eNFN.FIS
{
    public class TermCore
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public double X { get; set; }

        public double AccumulatedError { get; set; }
        
        public double ActivationCompetitionsWin { get; set; }

        public static TermCore Create(double x) => new TermCore {X = x};
        
        public override string ToString() => $"X: {X:0.0000} Activated: {ActivationCompetitionsWin} Error: {AccumulatedError}";
    }
}