using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    public class EndResultCalculator
    {
        private EndResult _results;
        private readonly int _numberOfPossibleRolls;

        public EndResultCalculator(Dictionary<int, double> permutations)
        {
            _results = new EndResult(new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels], new double[Constants.NumberOfCamels]);
            _numberOfPossibleRolls = Calculator.NumberOrPossibleRolls();

            CreateResults(permutations);
        }

        public EndResult GetEndProbabilities()
        {
            return _results;
        }

        public double GetProbabilityOfWin()
        {
            double sum = 0;
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                sum += _results.Winners[i];
            }

            return sum;
        }

        private void CreateResults(Dictionary<int, double> permutations)
        {
            var permutationIndexes = permutations.Keys.ToList();
            for (int i = 0; i < permutationIndexes.Count; i++)
            {
                var permutationIndex = permutationIndexes[i];
                var permutation = PermutationCalculator.Lookup[permutationIndex];
                _results.Winners[Array.IndexOf(permutation, 0)] += permutations[permutationIndex];
                _results.Losers[Array.IndexOf(permutation, Constants.NumberOfCamels - 1)] += permutations[permutationIndex];
            }
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                _results.Winners[i] /= _numberOfPossibleRolls;
                _results.Losers[i] /= _numberOfPossibleRolls;
                _results.ExpectedW8[i] = 9 * _results.Winners[i] - 1;
                _results.ExpectedW5[i] = 6 * _results.Winners[i] - 1;
                _results.ExpectedL8[i] = 9 * _results.Losers[i] - 1;
                _results.ExpectedL5[i] = 6 * _results.Losers[i] - 1;
            }
        }
    }
}
