using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    public static class PermutationCalculator
    {
        private static readonly int[] _powerArray;
        private static readonly int _numberOfPossibleRolls;

        static PermutationCalculator()
        {
            Identity = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                Identity[i] = i;
            }

            _powerArray = PowerArray();
            _numberOfPossibleRolls = Calculator.NumberOrPossibleRolls();

            Lookup = GenerateLookUp();
            CompositionMatrix = GenerateCompositionMatrix();
        }

        public static int[] Identity { get; }
        public static readonly Dictionary<int, int[]> Lookup;
        public static readonly Dictionary<int, Dictionary<int, int>> CompositionMatrix;

        public static int PermutationToInt(int[] permutation)
        {
            int integer = 0;
            var length = permutation.Length;
            for (int i = 0; i < length; i++)
            {
                integer += (permutation[i]) * (_powerArray[i]);
            }
            return integer;
        }

        public static int[] ComposePermutations(int[] permutation1, int[] permutation2)
        {
            var newPermutation = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                newPermutation[i] = permutation2[permutation1[i]];
            }

            return newPermutation;
        }

        public static Dictionary<int, double> MultiplyPermutations(Dictionary<int, double> statePermutations, Dictionary<int, double> rowPermutations)
        {
            var newPermutations = new Dictionary<int, double>();

            var statePermutationIndexes = statePermutations.Keys.ToList();
            var rowPermutationIndexes = rowPermutations.Keys.ToList();
            for (int i = 0; i < statePermutationIndexes.Count; i++)
            {
                var statePermutationIndex = statePermutationIndexes[i];
                var statePermutationValue = statePermutations[statePermutationIndex];

                for (int j = 0; j < rowPermutationIndexes.Count; j++)
                {
                    var rowPermutationIndex = rowPermutationIndexes[j];
                    var rowPermutationValue = rowPermutations[rowPermutationIndex];

                    var newPermutationIndex = CompositionMatrix[statePermutationIndex][rowPermutationIndex];
                    var newPermutationValue = rowPermutationValue * statePermutationValue / _numberOfPossibleRolls;

                    newPermutations.AddOrIncrement(newPermutationIndex, newPermutationValue);
                }
            }

            return newPermutations;
        }

        private static Dictionary<int, Dictionary<int,int>> GenerateCompositionMatrix()
        {
            var length = Lookup.Count;
            var allPermutations = Lookup.Values.ToList();
            var matrix = new Dictionary<int, Dictionary<int, int>>();
            for (int i = 0; i < length; i++)
            {
                var row = new Dictionary<int, int>();
                for (int j = 0; j < length; j++)
                {
                    var newPermutation = ComposePermutations(allPermutations[i], allPermutations[j]);
                    row.Add(PermutationToInt(allPermutations[j]), PermutationToInt(newPermutation));
                }
                matrix.Add(PermutationToInt(allPermutations[i]), row);
            }

            return matrix;
        }

        private static Dictionary<int, int[]> GenerateLookUp()
        {
            var dictionary = new Dictionary<int, int[]>();

            var length = Constants.NumberOfCamels;
            var c = new int[length];
            var array = new int[length];
            for (int j = 0; j < length; j++)
            {
                c[j] = 0;
                array[j] = j;
            }

            dictionary.Add(PermutationToInt(array),  array.CloneArray());

            int i = 0;
            while (i < length)
            {
                if (c[i] < i)
                {
                    if (i % 2 == 0)
                    {
                        array.Swap(0, i);
                    }
                    else
                    {
                        array.Swap(c[i], i);
                    }

                    dictionary.Add(PermutationToInt(array), array.CloneArray());
                    c[i]++;
                    i = 0;
                }
                else
                {
                    c[i] = 0;
                    i++;
                }
            }

            return dictionary;
        }

        private static int[] PowerArray()
        {
            var array = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                array[i] = (int)Math.Pow(Constants.NumberOfCamels, Constants.NumberOfCamels - i - 1);
            }
            return array;
        }
    }
}
