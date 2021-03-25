using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    public class LegResultCalculator
    {
        private LegResult _results;
        private readonly int _numberOfPossibleRolls;

        public LegResultCalculator(Dictionary<int, Dictionary<int, double>> rowState)
        {
            _results = new LegResult(new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels]);
            _numberOfPossibleRolls = Calculator.NumberOrPossibleRolls();

            GetLegResult(rowState);
        }

        public LegResult GetLegProbabilities()
        {
            return _results;
        }

        private void GetLegResult(Dictionary<int, Dictionary<int, double>> rowState)
        {
            var permutationsList = rowState.Values.ToList();
            for (int i = 0; i < permutationsList.Count; i++)
            {
                var permutations = permutationsList[i];
                var permutationIndexes = permutations.Keys.ToList();

                for (int j = 0; j < permutationIndexes.Count; j++)
                {
                    var permutationIndex = permutationIndexes[j];
                    var permutation = PermutationCalculator.Lookup[permutationIndex];
                    _results.First[Array.IndexOf(permutation, 0)] += permutations[permutationIndex];
                    _results.Second[Array.IndexOf(permutation, 1)] += permutations[permutationIndex];
                }
            }

            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                _results.First[i] /= _numberOfPossibleRolls;
                _results.Second[i] /= _numberOfPossibleRolls;
                _results.Expected5[i] = 6 * _results.First[i] + 2 * _results.Second[i] - 1;
                _results.Expected3[i] = 4 * _results.First[i] + 2 * _results.Second[i] - 1;
                _results.Expected2[i] = 3 * _results.First[i] + 2 * _results.Second[i] - 1;
            }
        }
    }
}
