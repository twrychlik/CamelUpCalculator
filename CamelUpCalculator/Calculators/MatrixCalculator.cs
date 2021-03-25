using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator.Calculators
{
    public class MatrixCalculator
    {

        public MatrixCalculator()
        {
        }

        public Dictionary<int, Dictionary<int, double>> GetNextState(Dictionary<int, Dictionary<int, double>> currentState, Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix)
        {
            var newState = new Dictionary<int, Dictionary<int, double>>();
            var statePositionIndexes = currentState.Keys.ToList();
            for (int i = 0; i < statePositionIndexes.Count; i++)
            {
                var positionIndex = statePositionIndexes[i];
                var statePermutations = currentState[positionIndex];

                var row = matrix[positionIndex];
                var rowPositionIndexes = row.Keys.ToList();

                for (int j = 0; j < rowPositionIndexes.Count; j++)
                {
                    var rowPositionIndex = rowPositionIndexes[j];
                    var rowPermutations = row[rowPositionIndex];

                    var newPermutations = PermutationCalculator.MultiplyPermutations(statePermutations, rowPermutations);

                    if (newState.ContainsKey(rowPositionIndex))
                    {
                        var newStatePermutations = newState[rowPositionIndex];
                        var newPermutationsIndexes = newPermutations.Keys.ToList();
                        for (int k = 0; k < newPermutationsIndexes.Count; k++)
                        {
                            var newPermutationIndex = newPermutationsIndexes[k];
                            var newPermutationValue = newPermutations[newPermutationIndex];
                            newStatePermutations.AddOrIncrement(newPermutationIndex, newPermutationValue);
                        }
                    }
                    else
                    {
                        newState.Add(rowPositionIndex, newPermutations);
                    }
                }
            }

            return newState;
        }

        internal Dictionary<int, Dictionary<int, Dictionary<int, double>>> SquareMatrix(Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix)
        {
            var nextMatrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();

            var positionIndexes = matrix.Keys.ToList();
            positionIndexes.Sort();
            for (int i = 0; i < positionIndexes.Count; i++)
            {
                Console.WriteLine((double)i / (double)positionIndexes.Count);
                var rowState = matrix[positionIndexes[i]];
                var nextRowState = GetNextState(rowState, matrix);
                nextMatrix.Add(positionIndexes[i], nextRowState);
            }

            return nextMatrix;
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> PartialSquareMatrix(Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix, int partialIndex, int totalPartials)
        {
            var partialMatrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            var positionIndexes = matrix.Keys.ToList();
            positionIndexes.Sort();

            var startIndex = (partialIndex) * (positionIndexes.Count / totalPartials);
            var endIndex = (partialIndex + 1) * (positionIndexes.Count / totalPartials);
            if (partialIndex + 1 == totalPartials)
            {
                endIndex = positionIndexes.Count;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                Console.WriteLine((double)i * totalPartials / (double)positionIndexes.Count);
                var rowState = matrix[positionIndexes[i]];
                var nextRowState = GetNextState(rowState, matrix);
                partialMatrix.Add(positionIndexes[i], nextRowState);
            }

            return partialMatrix;
        }

        public Dictionary<int, Dictionary<int, Dictionary<int, double>>> PartialUnitMatrix(Dictionary<int, Dictionary<int, Dictionary<int, double>>> matrix, Dictionary<int, Dictionary<int, Dictionary<int, double>>> partialMatrix, int partialIndex, int totalPartials)
        {
            var nextPartialMatrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            var positionIndexes = partialMatrix.Keys.ToList();
            positionIndexes.Sort();

            for (int i = 0; i < positionIndexes.Count; i++)
            {
                Console.Write((double)i / (double)positionIndexes.Count);
                Console.Write("     ");
                Console.Write(positionIndexes[i].ToString("X"));
                Console.Write(Environment.NewLine);
                var rowState = partialMatrix[positionIndexes[i]];
                var nextRowState = GetNextState(rowState, matrix);
                // Repeat 3 more times
                for (int j = 0; j < 3; j++)
                {
                    nextRowState = GetNextState(nextRowState, matrix);
                }
                nextPartialMatrix.Add(positionIndexes[i], nextRowState);
            }

            return nextPartialMatrix;
        }
    }
}
