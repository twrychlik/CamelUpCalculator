namespace CamelUpCalculator
{
    public struct LegBet
    {
        public LegBet(Colour camel, int bet)
        {
            Camel = camel;
            Bet = bet;
        }

        public Colour Camel;
        public int Bet;
    }
}
