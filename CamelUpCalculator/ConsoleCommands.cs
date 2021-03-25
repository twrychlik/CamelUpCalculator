using CamelUpCalculator.Calculators;

namespace CamelUpCalculator
{
    public static class ConsoleCommands
    {
        public static void Help()
        {
            Printer.Help();
        }

        public static void MakeMove(string move, GameState gameState)
        {
            if (move.Length != 2)
            {
                return;
            }

            var colour = move[0].ParseColour();
            if (colour == null)
            {
                Printer.Nope();
                return;
            }
            var number = int.Parse(move[1].ToString());

            if (!gameState.Dice.IsCamelLeft((int)colour))
            {
                Printer.Nope();
                return;
            }
            gameState.BoardState.MoveCamel(colour.GetValueOrDefault(), number);
            var resetRolls = gameState.Dice.RemoveCamel((int)colour);
            if (resetRolls)
            {
                gameState.BoardState.ClearModifiers();
                gameState.Bets.ClearLegBets();

            }
            if (gameState.BoardState.IsWon)
            {
                Printer.PrintWinner(gameState.BoardState.Winner);
            }
            Printer.PrintBoard(gameState);
        }

        public static void RollSet(GameState gameState)
        {
            var rollSet = gameState.Dice.GetRollSet();
            for (int i = 0; i < rollSet.Length; i++)
            {
                gameState.BoardState.MoveCamel(rollSet[i]);
                gameState.Dice.RemoveCamel(rollSet[i].Camel);
                if (gameState.BoardState.IsWon)
                {
                    Printer.PrintWinner(gameState.BoardState.Winner);
                    break;
                }
            }
            gameState.BoardState.ClearModifiers();
            gameState.Bets.ClearLegBets();
            Printer.PrintBoard(gameState);
        }

        public static void Roll(GameState gameState)
        {
            var roll = gameState.Dice.GetRoll();
            gameState.BoardState.MoveCamel(roll);
            gameState.Dice.RemoveCamel(roll.Camel);
            if (gameState.BoardState.IsWon)
            {
                Printer.PrintWinner(gameState.BoardState.Winner);
            }

            if (gameState.Dice.CamelsLeft.Count == Constants.NumberOfCamels)
            {
                gameState.BoardState.ClearModifiers();
                gameState.Bets.ClearLegBets();
            }
            Printer.PrintBoard(gameState);
        }

        public static void LegProbability(PositionSimulator positionSimulator, BoardState boardState, Dice dice)
        {
            var state2 = boardState.GetState();
            if (state2.Position == null)
            {
                Printer.Sorry();
                return;
            }

            var results = positionSimulator.GetLegResults(state2, dice);
            Printer.PrintLegResult(results);
        }

        public static void EndProbability(PositionSimulator positionSimulator, BoardState boardState, Dice dice)
        {
            var state2 = boardState.GetState();
            if (state2.Position == null)
            {
                Printer.Sorry();
                return;
            }

            var results = positionSimulator.GetEndResults(state2, dice);
            Printer.PrintEndResult(results);
        }

        public static void AllProbabilities(PositionSimulator positionSimulator, GameState gameState)
        {
            var state2 = gameState.BoardState.GetState();
            if (state2.Position == null)
            {
                Printer.Nope();
                return;
            }

            var allresults = positionSimulator.GetAllResults(gameState);
            Printer.PrintLegResult(allresults.LegResult);
            Printer.PrintEndResult(allresults.EndResult);
            Printer.PrintModifierResults(allresults);
        }

        public static void ModifierProbability(PositionSimulator positionSimulator, GameState gameState)
        {
            var state2 = gameState.BoardState.GetState();
            if (state2.Position == null)
            {
                Printer.Nope();
                return;
            }

            var results = positionSimulator.GetAllResults(gameState);
            Printer.PrintModifierResults(results);
        }

        public static void PlaceMirage(string square, GameState gameState)
        {
            if (!int.TryParse(square, out var number))
            {
                Printer.Nope();
            }

            if (!gameState.BoardState.PlaceModifier(number, Modifier.Mirage))
            {
                Printer.Nope();
            }

            Printer.PrintBoard(gameState);
        }

        public static void PlaceOasis(string square, GameState gameState)
        {
            if (!int.TryParse(square, out var number))
            {
                Printer.Nope();
            }

            if (!gameState.BoardState.PlaceModifier(number, Modifier.Oasis))
            {
                Printer.Nope();
            }
            
            Printer.PrintBoard(gameState);
        }

        public static void PlaceLegBet(string arg, GameState gameState)
        {
            if (arg.Length != 2)
            {
                Printer.Nope();
                return;
            }

            var colour = arg[0].ParseColour();
            int.TryParse(arg[1].ToString(), out int bet);

            if ((colour == null) || (bet != 5 && bet != 3 && bet != 2))
            {
                Printer.Nope();
                return;
            }

            var success = gameState.Bets.AddLegBet(new LegBet(colour.GetValueOrDefault(), bet));
            if (!success)
            {
                Printer.Nope();
                return;
            }
            Printer.PrintBoard(gameState);
        }

        public static void PlaceEndBet(string winLose, string colourPlace, GameState gameState)
        {
            var colour = colourPlace[0].ParseColour();
            int.TryParse(colourPlace[1].ToString(), out int place);
            var win = winLose.ToString() == "win";
            if (colour == null || !(place > 0 && place < 9) || (winLose.ToString() != "win" && winLose.ToString() != "lose"))
            {
                Printer.Nope();
                return;
            }

            var success = gameState.Bets.AddEndBet(new EndBet(colour.GetValueOrDefault(), win, place));
            if (!success)
            {
                Printer.Nope();
                return;
            }
            Printer.PrintBoard(gameState);
        }

        public static void GetTopPlays(PositionSimulator positionSimulator, GameState gameState, string number)
        {
            int.TryParse(number, out int count);
            var allresults = positionSimulator.GetAllResults(gameState);
            var allResultsCalculator = new AllResultsCalculator(allresults);
            var topPlays = allResultsCalculator.GetTopPlays(count);
            Printer.PrintTopPlays(topPlays);
        }

        public static void Reset(GameState gameState)
        {
            gameState.BoardState.Reset();
            gameState.Dice.Reset();
            gameState.Bets.Reset();
            Printer.PrintBoard(gameState);
        }
    }
}
