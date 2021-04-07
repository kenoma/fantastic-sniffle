using System;
using System.Reflection.Metadata.Ecma335;

namespace eNFN.FIS
{
    public class TermCore
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public double X { get; set; }

        public double AccumulatedError { get; set; }
        
        public double ActivationCompetitionsFailedInARow { get; set; }

        //public double InitialError { get; set; }

        //public double ErrorRate => AccumulatedError / InitialError;

        // public static TermCore Create(double x, double initialError) =>
        //     new() {X = x, InitialError = initialError};

        public static TermCore Create(double x) => new() {X = x};

        public override string ToString() =>
            $"X: {X:0.0000} Failed competitions: {ActivationCompetitionsFailedInARow} Error: {AccumulatedError}";
    }
}