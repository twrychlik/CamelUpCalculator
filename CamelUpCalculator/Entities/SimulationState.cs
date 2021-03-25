using System.Collections.Generic;

namespace CamelUpCalculator
{
    public struct SimulationState
    {
        public SimulationState(int[] position, int[] permutation, List<int> modifiers)
        {
            Position = position;
            Permutation = permutation;
            Modifiers = modifiers;
        }

        public int[] Position { get; set; }
        public int[] Permutation { get; set; }
        public List<int> Modifiers { get; set; }
    }
}
