using System.Diagnostics;

namespace CamelUpCalculator
{
    [DebuggerDisplay("{DebugDisplay}")]
    public class Roll
    {
        public string DebugDisplay => $"{this.Camel}, {this.Number }";
        public int Camel { get; }
        public int Number { get; }

        public Roll(int camel, int number)
        {
            Camel = camel;
            Number = number;
        }
    }
}
