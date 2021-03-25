using System;
using System.Collections.Generic;

namespace CamelUpCalculator
{
    public struct AllResults
    {
        public AllResults(LegResult legResult, EndResult endResult, Tuple<List<ModifierResult>, List<ModifierResult>> modifierTotals)
        {
            LegResult = legResult;
            EndResult = endResult;
            ModifierTotals = modifierTotals;
        }

        public LegResult LegResult;
        public EndResult EndResult;
        public Tuple<List<ModifierResult>, List<ModifierResult>> ModifierTotals;
    }
}
