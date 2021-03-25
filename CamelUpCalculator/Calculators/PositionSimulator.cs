using System;
using System.Collections.Generic;
using System.Linq;
using CamelUpCalculator.Calculators;

namespace CamelUpCalculator
{
    public class PositionSimulator
    {
        private readonly int _numberOfPossibleRolls;
        private RollsetCalculator _rollsetCalculator;
        private MatrixCalculator _matrixCalculator;
        private MatrixRepository _matrixRepository;

        public PositionSimulator()
        {
            _numberOfPossibleRolls = Calculator.NumberOrPossibleRolls();
            _rollsetCalculator = new RollsetCalculator();
            _matrixCalculator = new MatrixCalculator();
            _matrixRepository = new MatrixRepository(_matrixCalculator, GetInitialMarkovMatrix);
        }

        public LegResult GetLegResults(SimulationState state, Dice dice)
        {
            var rollsets = _rollsetCalculator.CreateAllRollSets(dice.CamelsLeft);
            var rowState = GetAllPossibleLegPositions(state, rollsets, out _);
            var legResults = new LegResultCalculator(rowState);

            return legResults.GetLegProbabilities();
        }

        public EndResult GetEndResults(SimulationState state, Dice dice)
        {
            var rowState = GetRowFromState(state);

            if (dice.CamelsLeft.Count != Constants.NumberOfCamels || state.Modifiers.Count != 0)
            {
                var rollsets = _rollsetCalculator.CreateAllRollSets(dice.CamelsLeft);
                rowState = GetAllPossibleLegPositions(state, rollsets, out _);
            }

            var winCell = GetWinCell(rowState);
            var winResults = new EndResultCalculator(winCell);
            var winningProbability = winResults.GetProbabilityOfWin();
            while (winningProbability < Constants.WinningAccuracy)
            {
                rowState = _matrixCalculator.GetNextState(rowState, _matrixRepository.EndMatrix);

                winResults = new EndResultCalculator(GetWinCell(rowState));
                winningProbability = winResults.GetProbabilityOfWin();
            }

            return winResults.GetEndProbabilities();
        }

        public Tuple<List<ModifierResult>, List<ModifierResult>> GetModifierResults(SimulationState state, Dice dice, List<int> availableSquares)
        {
            var rollsets = _rollsetCalculator.CreateAllRollSets(dice.CamelsLeft);
            var rollsetCount = Calculator.NumberOfPossibleRolls(dice.CamelsLeft.Count);
            var modifierResultCalculator = new ModifierResultCalculator(rollsetCount, null);

            foreach (var square in availableSquares)
            {
                state.Modifiers.Add(square);
                GetAllPossibleLegPositions(state, rollsets, out var modifierTotals);
                modifierResultCalculator.AddOasisResult(square, modifierTotals[Constants.NumberOfSquares + square - 1]);
                state.Modifiers.Remove(square);
            }

            foreach (var square in availableSquares)
            {
                state.Modifiers.Add(-1 * square);
                GetAllPossibleLegPositions(state, rollsets, out var modifierTotals);
                modifierResultCalculator.AddMirageResult(square, modifierTotals[square - 1]);
                state.Modifiers.Remove(-1 * square);
            }

            var mirageTotals = modifierResultCalculator.GetMirageExpectedTotals();
            var oasisTotals = modifierResultCalculator.GetOasisExpectedTotals();
            return new Tuple<List<ModifierResult>, List<ModifierResult>>(mirageTotals, oasisTotals);
        }

        public AllResults GetAllResults(GameState gameState)
        {
            var simulationState = gameState.BoardState.GetState();
            var baseResults = SimulateToEnd(simulationState, gameState.Dice);
            var rollsetCount = Calculator.NumberOfPossibleRolls(gameState.Dice.CamelsLeft.Count);
            var modifierResultCalculator = new ModifierResultCalculator(rollsetCount, gameState.Bets);

            var availableSquares = gameState.BoardState.GetAvailableSquares();
            foreach (var square in availableSquares)
            {
                simulationState.Modifiers.Add(square);
                var simulatedResults = SimulateToEnd(simulationState, gameState.Dice);
                modifierResultCalculator.AddOasisResult(square, baseResults, simulatedResults);
                simulationState.Modifiers.Remove(square);
            }

            foreach (var square in availableSquares)
            {
                simulationState.Modifiers.Add(-1 * square);
                var simulatedResults = SimulateToEnd(simulationState, gameState.Dice);
                modifierResultCalculator.AddMirageResult(square, baseResults, simulatedResults);
                simulationState.Modifiers.Remove(-1 * square);
            }

            var mirageTotals = modifierResultCalculator.GetMirageExpectedTotals();
            var oasisTotals = modifierResultCalculator.GetOasisExpectedTotals();
            var modifierResult = new Tuple<List<ModifierResult>, List<ModifierResult>>(mirageTotals, oasisTotals);
            return new AllResults(baseResults.LegResult, baseResults.EndResult, modifierResult);
        }

        private BaseResults SimulateToEnd(SimulationState simulationState, Dice dice)
        {
            // Get leg results
            var rollsets = _rollsetCalculator.CreateAllRollSets(dice.CamelsLeft);
            var rowState = GetAllPossibleLegPositions(simulationState, rollsets, out var modifierTotals);
            var legResult = new LegResultCalculator(rowState);

            // Get end results
            rowState = _matrixCalculator.GetNextState(rowState, _matrixRepository.EndMatrix);
            var winCell = GetWinCell(rowState);
            var endResult = new EndResultCalculator(winCell);

            return new BaseResults(legResult.GetLegProbabilities(), endResult.GetEndProbabilities(), modifierTotals);
        }

        private Dictionary<int, double> GetWinCell(Dictionary<int, Dictionary<int, double>> rowState)
        {
            return rowState.ContainsKey(-1)
                   ? rowState[-1]
                   : new Dictionary<int, double> { { PermutationCalculator.PermutationToInt(PermutationCalculator.Identity), 0.0 } };
        }

        private Dictionary<int, Dictionary<int, double>> GetRowFromState(SimulationState state)
        {
            var row = new Dictionary<int, Dictionary<int, double>>();
            var positionIndex = PositionsCalculator.PositionToInt(state.Position);

            var permutations = new Dictionary<int, double>();
            var permutationIndex = PermutationCalculator.PermutationToInt(state.Permutation);
            permutations.Add(permutationIndex, _numberOfPossibleRolls);
            row.Add(positionIndex, permutations);

            return row;
        }

        internal Dictionary<int, Dictionary<int, Dictionary<int, double>>> GetInitialMarkovMatrix()
        {
            var allPositions = PositionsCalculator.GetAllPositions().Values.ToArray();
            var matrix = new Dictionary<int, Dictionary<int, Dictionary<int, double>>>();
            var rollsets = _rollsetCalculator.MaxRollSet;
            var permutation = PermutationCalculator.Identity.CloneArray();
            var modifiers = new List<int>();

            var length = allPositions.Length;
            for (int i = 0; i < length; i++)
            {
                Console.WriteLine((double)i/(double)length);
                var position = allPositions[i];
                var state = new SimulationState(position, permutation, modifiers);
                var row = GetAllPossibleLegPositions(state, rollsets, out _);
                var positionIndex = PositionsCalculator.PositionToInt(position);
                matrix.Add(positionIndex, row);
            }

            return matrix;
        }

        private Dictionary<int, Dictionary<int, double>> GetAllPossibleLegPositions(SimulationState state, List<List<Roll>> rollsets, out double[] modifierTotals)
        {
            var row = new Dictionary<int, Dictionary<int, double>>();
            modifierTotals = new double[2 * Constants.NumberOfSquares];
            var probabilityRatio = Calculator.NumberOrPossibleRolls() / Calculator.NumberOfPossibleRolls(rollsets[0].Count);
            foreach (var rollset in rollsets)
            {
                var nextState = new SimulationState(state.Position.CloneArray(), state.Permutation.CloneArray(), state.Modifiers.CloneList());
                for (int i = 0; i < rollset.Count; i++)
                {
                    nextState = ApplyRoll(nextState, rollset[i], modifierTotals);
                }

                var positionIndex = PositionsCalculator.PositionToInt(nextState.Position);
                var permutationIndex = PermutationCalculator.PermutationToInt(nextState.Permutation);
                if (row.ContainsKey(positionIndex))
                {
                    if (row[positionIndex].ContainsKey(permutationIndex))
                    {
                        row[positionIndex][permutationIndex] += probabilityRatio;
                    }
                    else
                    {
                        row[positionIndex].Add(permutationIndex, probabilityRatio);
                    }
                }
                else
                {
                    var permutations = new Dictionary<int, double>
                    {
                        { permutationIndex, probabilityRatio }
                    };
                    row.Add(positionIndex, permutations);
                }
            }

            return row;
        }

        private SimulationState ApplyRoll(SimulationState state, Roll roll, double[] modifierTotals)
        {
            // Check if already won
            if (state.Position[0] == -1)
            {
                return state;
            }

            var bottomOfStack = state.Permutation[roll.Camel];
            var topOfStack = GetTopOfStack(state.Position, bottomOfStack);
            var stackSize = bottomOfStack - topOfStack + 1;
            var newPositionValue = GetNewPositionValue(state, bottomOfStack, roll.Number, modifierTotals, out bool insertBelow);
            var insertPosition = GetInsertPosition(state.Position, newPositionValue, topOfStack, bottomOfStack, insertBelow);

            var newPosition = new int[Constants.NumberOfCamels];
            var newPermutation = new int[Constants.NumberOfCamels];

            // Stack doesn't move
            if (insertPosition == topOfStack)
            {
                // Increment stack
                for (int i = topOfStack; i < topOfStack + stackSize; i++)
                {
                    newPosition = state.Position;
                    newPosition[i] = newPositionValue;
                    newPermutation = PermutationCalculator.Identity.CloneArray();
                }
            }
            // Stack moves up
            else if (insertPosition < topOfStack)
            {
                // Copy start of array
                for (int i = 0; i < insertPosition; i++)
                {
                    newPosition[i] = state.Position[i];
                    newPermutation[i] = i;
                }

                // Add stack
                for (int i = insertPosition; i < insertPosition + stackSize; i++)
                {
                    newPosition[i] = newPositionValue;
                    newPermutation[i + topOfStack - insertPosition] = i;
                }

                // Move displaced camels down
                for (int i = insertPosition + stackSize; i < bottomOfStack + 1; i++)
                {
                    newPosition[i] = state.Position[i - stackSize];
                    newPermutation[i - stackSize] = i;
                }

                // Copy end of array
                for (int i = bottomOfStack + 1; i < Constants.NumberOfCamels; i++)
                {
                    newPosition[i] = state.Position[i];
                    newPermutation[i] = i;
                }
            }
            else // Stack moves down
            {
                // Copy start of array
                for (int i = 0; i < topOfStack; i++)
                {
                    newPosition[i] = state.Position[i];
                    newPermutation[i] = i;
                }

                // Move displaced camels up
                for (int i = topOfStack; i < insertPosition - stackSize; i++)
                {
                    newPosition[i] = state.Position[i + stackSize];
                    newPermutation[i + stackSize] = i;
                }

                // Move stack down
                for (int i = insertPosition - stackSize; i < insertPosition; i++)
                {
                    newPosition[i] = newPositionValue;
                    newPermutation[i - insertPosition + bottomOfStack + 1] = i;
                }

                // Copy end of array
                for (int i = insertPosition; i < Constants.NumberOfCamels; i++)
                {
                    newPosition[i] = state.Position[i];
                    newPermutation[i] = i;
                }
            }

            // Check for win
            if (newPositionValue > Constants.NumberOfSquares)
            {
                newPosition = PositionsCalculator.VictoryPosition.CloneArray();
            }

            state.Position = newPosition;
            state.Permutation = PermutationCalculator.ComposePermutations(state.Permutation, newPermutation);

            return state;
        }

        private int GetTopOfStack(int[] position, int bottomOfStack)
        {
            for (int i = 0; i < bottomOfStack; i++)
            {
                if (position[i] == position[bottomOfStack])
                {
                    return i;
                }
            }

            return bottomOfStack;
        }

        private int GetNewPositionValue(SimulationState state, int bottomOfStack, int roll, double[] modifierTotals, out bool insertBelow)
        {
            insertBelow = false;
            var newSquare = state.Position[bottomOfStack] + roll;
            if (state.Modifiers.Count != 0)
            {
                if (state.Modifiers.Contains(newSquare))
                {
                    modifierTotals[Constants.NumberOfSquares + newSquare - 1]++;
                    newSquare++;
                }
                else if (state.Modifiers.Contains(newSquare * -1))
                {
                    modifierTotals[newSquare - 1]++;
                    newSquare--;
                    insertBelow = true;
                }
            }

            return newSquare;
        }

        private int GetInsertPosition(int[] position, int newPositionValue, int topOfStack, int bottomOfStack, bool insertBelow)
        {
            if (insertBelow)
            {
                for (int i = Constants.NumberOfCamels - 1; i >= 0; i--)
                {
                    if (position[i] >= newPositionValue)
                    {
                        // Don't insert stack after itself
                        if (i == bottomOfStack)
                        {
                            return topOfStack;
                        }

                        return i + 1;
                    }
                }

                return 0;
            }
            else
            {
                for (int i = 0; i < Constants.NumberOfCamels; i++)
                {
                    if (position[i] <= newPositionValue)
                    {
                        return i;
                    }
                }

                return Constants.NumberOfCamels;
            }
        }
    }
}