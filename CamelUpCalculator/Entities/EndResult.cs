namespace CamelUpCalculator
{
    public struct EndResult
    {
        public EndResult(double[] winners, double[] losers, double[] expectedW8, double[] expectedW5, double[] expectedL8, double[] expectedL5)
        {
            Winners = winners;
            Losers = losers;
            ExpectedW8 = expectedW8;
            ExpectedW5 = expectedW5;
            ExpectedL8 = expectedL8;
            ExpectedL5 = expectedL5;
        }

        public double[] Winners;
        public double[] Losers;
        public double[] ExpectedW8;
        public double[] ExpectedW5;
        public double[] ExpectedL8;
        public double[] ExpectedL5;
    }
}
