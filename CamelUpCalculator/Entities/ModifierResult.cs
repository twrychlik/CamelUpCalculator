namespace CamelUpCalculator
{
    public struct ModifierResult
    {
        public ModifierResult(int square, double expectedTotal)
        {
            Square = square;
            ExpectedTotal = expectedTotal;
        }

        public int Square;

        public double ExpectedTotal;
    }
}