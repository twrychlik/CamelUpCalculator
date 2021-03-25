namespace CamelUpCalculator
{
    public struct LegResult
    {
        public LegResult(double[] first, double[] second, double[] expected5, double[] expected3, double[] expected2)
        {
            First = first;
            Second = second;
            Expected5 = expected5;
            Expected3 = expected3;
            Expected2 = expected2;
        }

        public double[] First;
        public double[] Second;
        public double[] Expected5;
        public double[] Expected3;
        public double[] Expected2;
    }
}
