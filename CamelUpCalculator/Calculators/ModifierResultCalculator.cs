using System.Collections.Generic;

namespace CamelUpCalculator
{
    public class ModifierResultCalculator
    {
        private List<ModifierResult> _oasisResults;
        private List<ModifierResult> _mirageResults;

        private int _rollsetCount;

        private Bets _bets;

        public ModifierResultCalculator(int numberOfPossibleRollsets, Bets bets)
        {
            _oasisResults = new List<ModifierResult>();
            _mirageResults = new List<ModifierResult>();
            _rollsetCount = numberOfPossibleRollsets;
            _bets = bets;
        }

        public void AddOasisResult(int square, double expectedIncome)
        {
            expectedIncome /= _rollsetCount;
            _oasisResults.Add(new ModifierResult(square, expectedIncome));
        }

        public void AddOasisResult(int square, BaseResults baseResult, BaseResults simulatedResult)
        {
            var directIncome = simulatedResult.ModifierTotals[Constants.NumberOfSquares + square - 1] /= _rollsetCount;
            var expectedIncome = GetExpectedIncome(directIncome, baseResult, simulatedResult);
            _oasisResults.Add(new ModifierResult(square, expectedIncome));
        }

        public void AddMirageResult(int square, double expectedIncome)
        {
            expectedIncome /= _rollsetCount;
            _mirageResults.Add(new ModifierResult(square, expectedIncome));
        }

        public void AddMirageResult(int square, BaseResults baseResult, BaseResults simulatedResult)
        {
            var directIncome = simulatedResult.ModifierTotals[square - 1] /= _rollsetCount;
            var expectedIncome = GetExpectedIncome(directIncome, baseResult, simulatedResult);
            _mirageResults.Add(new ModifierResult(square, expectedIncome));
        }

        public List<ModifierResult> GetMirageExpectedTotals()
        {
            return _mirageResults;
        }

        public List<ModifierResult> GetOasisExpectedTotals()
        {
            return _oasisResults;
        }

        private double GetExpectedIncome(double directIncome, BaseResults baseResult, BaseResults simulatedResult)
        {
            var legBetGain = 0.0;
            var endBetGain = 0.0;
            foreach (var legBet in _bets?.GetLegBets())
            {
                var baseExpectation = 0.0;
                var simulatedExpectation = 0.0;
                switch (legBet.Bet)
                {
                    case (5):
                        baseExpectation = baseResult.LegResult.Expected5[(int)legBet.Camel];
                        simulatedExpectation = simulatedResult.LegResult.Expected5[(int)legBet.Camel];
                        break;
                    case (3):
                        baseExpectation = baseResult.LegResult.Expected3[(int)legBet.Camel];
                        simulatedExpectation = simulatedResult.LegResult.Expected3[(int)legBet.Camel];
                        break;
                    case (2):
                        baseExpectation = baseResult.LegResult.Expected2[(int)legBet.Camel];
                        simulatedExpectation = simulatedResult.LegResult.Expected2[(int)legBet.Camel];
                        break;
                }

                legBetGain += simulatedExpectation - baseExpectation;
            }

            foreach (var endBet in _bets?.GetEndBets())
            {
                var baseExpectation = 0.0;
                var simulatedExpectation = 0.0;
                if (endBet.Win)
                {
                    if (endBet.Place > 2)
                    {
                        baseExpectation = baseResult.EndResult.ExpectedW8[(int)endBet.Camel];
                        simulatedExpectation = simulatedResult.EndResult.ExpectedW8[(int)endBet.Camel];
                    }
                    else
                    {
                        baseExpectation = baseResult.EndResult.ExpectedW5[(int)endBet.Camel];
                        simulatedExpectation = simulatedResult.EndResult.ExpectedW5[(int)endBet.Camel];
                    }
                }
                else
                {
                    if (endBet.Place > 2)
                    {
                        baseExpectation = baseResult.EndResult.ExpectedL8[(int)endBet.Camel];
                        simulatedExpectation = simulatedResult.EndResult.ExpectedL8[(int)endBet.Camel];
                    }
                    else
                    {
                        baseExpectation = baseResult.EndResult.ExpectedL5[(int)endBet.Camel];
                        simulatedExpectation = simulatedResult.EndResult.ExpectedL5[(int)endBet.Camel];
                    }
                }

                endBetGain += simulatedExpectation - baseExpectation;
            }

            return directIncome + legBetGain + endBetGain;
        }
    }
}
