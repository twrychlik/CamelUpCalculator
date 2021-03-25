using System;

namespace CamelUpCalculator
{
    [Serializable]
    public class Camel
    {
        public Colour Colour { get; }
        public int Square { get; set; }
        public int Height { get; set; }

        public Camel(Colour colour)
        {
            Colour = colour;
            Square = 0;
            Height = -1;
        }

        public void Reset()
        {
            Square = 0;
            Height = -1;
        }
    }
}
