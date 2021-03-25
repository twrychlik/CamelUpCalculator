using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator.Calculators
{
    public class AllResultsCalculator
    {
        private AllResults _allResults;
        private string _legString = " to win leg";

        public AllResultsCalculator(AllResults allResults)
        {
            _allResults = allResults;
        }

        public List<Tuple<string, double>> GetTopPlays(int number)
        {
            var allPlays = new List<Tuple<string, double>>();

            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                var camel = " " + ((Colour) i).ToString();

                allPlays.Add(new Tuple<string, double>(camel + _legString + " (5):", _allResults.LegResult.Expected5[i]));
                allPlays.Add(new Tuple<string, double>(camel + _legString + " (3):", _allResults.LegResult.Expected3[i]));
                allPlays.Add(new Tuple<string, double>(camel + _legString + " (2):", _allResults.LegResult.Expected2[i]));

                allPlays.Add(new Tuple<string, double>(camel + " to win (8)", _allResults.EndResult.ExpectedW8[i]));
                allPlays.Add(new Tuple<string, double>(camel + " to win (5)", _allResults.EndResult.ExpectedW5[i]));
                allPlays.Add(new Tuple<string, double>(camel + " to lose (8)", _allResults.EndResult.ExpectedL8[i]));
                allPlays.Add(new Tuple<string, double>(camel + " to lose (5)", _allResults.EndResult.ExpectedL5[i]));
            }

            foreach (var mirage in _allResults.ModifierTotals.Item1)
            {
                allPlays.Add(new Tuple<string, double>(" Mirage on tile " + mirage.Square, mirage.ExpectedTotal));
            }

            foreach (var oasis in _allResults.ModifierTotals.Item2)
            {
                allPlays.Add(new Tuple<string, double>(" Oasis on tile "+ oasis.Square, oasis.ExpectedTotal));
            }

            var topPlays = allPlays.OrderByDescending(x => x.Item2).ToList().GetRange(0, Math.Min(number, allPlays.Count()));
            return topPlays;
        }
    }
}
