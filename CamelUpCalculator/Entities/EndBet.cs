
namespace CamelUpCalculator
{
    public struct EndBet
    {
        public EndBet(Colour camel, bool win, int place)
        {
            Camel = camel;
            Place = place;
            Win = win;
        }

        public Colour Camel;
        public int Place;
        public bool Win;
    }
}
