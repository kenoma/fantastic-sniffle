using System;
using System.Collections.Generic;

namespace eNFN.eANFIS
{
    public interface IRuleset
    {
        double GetInferenceForRule(IReadOnlyList<double> inputX, IReadOnlyList<Guid> termIds);

        void BackpropError(IReadOnlyList<double> inputX, IReadOnlyList<Guid> termIds, double error, double firing);
        
        void EliminateRules(int layer, Guid eliminatingTerm);
    }
}