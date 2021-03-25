namespace CamelUpCalculator
{
    public struct BaseResults
    {
        public BaseResults(LegResult legResult, EndResult endResult, double[] modifierTotals)
        {
            LegResult = legResult;
            EndResult = endResult;
            ModifierTotals = modifierTotals;
        }

        public LegResult LegResult;
        public EndResult EndResult;
        public double[] ModifierTotals;
    }
}
