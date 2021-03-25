using System;
using System.Collections.Generic;
using System.Linq;
using Console = System.Console;

namespace CamelUpCalculator
{
    public static class Printer
    {
        public static void Help()
        {
            Console.WriteLine("exit                 Closes the window.");
            Console.WriteLine("help                 Displays and explains the available commands.");
            Console.WriteLine("reset                Resets the game to its starting position");
            Console.WriteLine();
            Console.WriteLine("[camel][roll]        Moves a camel forward. e.g. \"b3\" moves the blue camel three tiles.");
            Console.WriteLine("mirage [tile]        Places a mirage on a tile. e.g. \"mirage 12\" places a mirage on tile 12");
            Console.WriteLine("oasis [tile]         Places a mirage on a tile. e.g. \"oasis 12\" places an oasis on tile 12");
            Console.WriteLine();
            Console.WriteLine("leg [camel][value]   Places a leg bet on a camel for a card value. e.g. \"leg y3\" places a bet on yellow, with max return of 3.");
            Console.WriteLine("win [camel][place]   Places a win bet on a camel for one's place. e.g. \"win w2\" means you are the 2nd bet for white to win.");
            Console.WriteLine("lose [camel][place]  Places a lose bet on a camel for one's place. e.g. \"lose w2\" means you are the 2nd bet for white to lose.");
            Console.WriteLine();
            Console.WriteLine("all                  Displays all expected betting returns.");
            Console.WriteLine("leg result           Displays expected betting returns on placing leg bets.");
            Console.WriteLine("end result           Displays expected betting returns on placing end win or lose bets.");
            Console.WriteLine("modifiers            Displays expected betting returns on placing mirages or oasies.");
            Console.WriteLine("top [number]         Displays plays with the best expected returns. e.g. top 5 displays the best 5 bets to make.");
            Console.WriteLine();
            Console.WriteLine("roll                 Makes a random legal camel move.");
            Console.WriteLine("rollset              Carries out a full random leg, or if in progress, completes a leg.");
        }

        public static void Nope()
        {
            Console.WriteLine("Don't understand - type \"help\" for help.");
        }

        public static void Sorry()
        {
            Console.WriteLine("Sorry, with hindsight I'd have adding the null starting position as displaying probabilities. But given you can't make bets, I omitted it");
        }

        public static void PrintBoard(GameState gameState)
        {
            var boardState = gameState.BoardState;
            for (int i = Constants.NumberOfCamels; i > 1; i--)
            {
                for (int j = 1; j < boardState.Board.Length; j++)
                {
                    if (boardState.Board[j].Camels.Count >= i)
                    {
                        SetBrightPrintColour(boardState.Board[j].Camels[i - 1]);
                        Console.Write(boardState.Board[j].Camels[i - 1].ToInitialString());
                        ResetColour();
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.Write(Environment.NewLine);
            }

            for (int j = 1; j < boardState.Board.Length; j++)
            {
                var square = boardState.Board[j];
                if (square.Camels.Count != 0)
                {
                    SetBrightPrintColour(square.Camels[0]);
                    Console.Write(square.Camels[0].ToInitialString());
                    ResetColour();
                }
                else if (square.Modifier != Modifier.None)
                {
                    Console.Write(square.Modifier.ToFriendlyString());
                }
                else
                {
                    Console.Write("_");
                }
            }

            if (gameState.Bets.GetLegBets().Any())
            {
                Console.Write(" Leg Bets: ");
                foreach (var legBet in gameState.Bets.GetLegBets())
                {
                    SetBrightPrintColour(legBet.Camel);
                    Console.Write(legBet.Camel.ToInitialString());
                    WriteLegBetValue(legBet.Bet);
                    Console.Write(" ");
                }
            }

            if (gameState.Bets.GetEndBets().Any())
            {
                Console.Write(" End Bets: ");
                foreach (var endBet in gameState.Bets.GetEndBets())
                {
                    SetBrightPrintColour(endBet.Camel);
                    Console.Write(endBet.Camel.ToInitialString());
                    WriteEndBetValue(endBet);
                    Console.Write(" ");
                }
            }

            Console.Write(Environment.NewLine);

            for (int j = 1; j < boardState.Board.Length; j++)
            {
                Console.Write(j > 9 ? j - 10 : j);
            }

            Console.Write(" Dice left:");
            foreach (var colour in gameState.Dice.CamelsLeft)
            {
                SetBrightPrintColour((Colour)colour);
                Console.Write(((Colour)colour).ToInitialString());
                ResetColour();
            }

            Console.Write(Environment.NewLine);
        }

        public static void PrintWinner(Colour winner)
        {
            Console.WriteLine(winner.ToString() + " wins!");
        }

        public static void PrintLegResult(LegResult results)
        {
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                var colour = ((Colour)i).ToFriendlyString();
                var firstPercentage = results.First[i].ToPercentageString();
                var secondPercentage = results.Second[i].ToPercentageString();
                SetBrightPrintColour((Colour)i);
                Console.Write(colour);
                ResetColour();
                Console.Write(" 1st:" + firstPercentage + " 2nd:" + secondPercentage + " E5: ");
                WriteExpectedString(results.Expected5[i]);
                Console.Write(" E3: ");
                WriteExpectedString(results.Expected3[i]);
                Console.Write(" E2: ");
                WriteExpectedString(results.Expected2[i]);
                Console.Write(Environment.NewLine);
            }
            Console.Write(Environment.NewLine);
        }

        public static void PrintEndResult(EndResult results)
        {
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                var colour = ((Colour)i).ToFriendlyString();
                var winPercentage = results.Winners[i].ToPercentageString();
                var losePercentage = results.Losers[i].ToPercentageString();
                SetBrightPrintColour((Colour)i);
                Console.Write(colour);
                ResetColour();
                Console.Write(" First:" + winPercentage + " Last:" + losePercentage + " W8: ");
                WriteExpectedString(results.ExpectedW8[i]);
                Console.Write(" W5: ");
                WriteExpectedString(results.ExpectedW5[i]);
                Console.Write(" L8: ");
                WriteExpectedString(results.ExpectedL8[i]);
                Console.Write(" L5: ");
                WriteExpectedString(results.ExpectedL5[i]);
                Console.Write(Environment.NewLine);
            }
            Console.Write(Environment.NewLine);
        }

        public static void PrintModifierResults(AllResults allResults)
        {
            var results = allResults.ModifierTotals;
            Console.Write("         ");
            for (int i = 0; i < results.Item1.Count; i++)
            {
                var space = results.Item1[i].Square < 10 ? "    " : "   ";
                Console.Write(results.Item1[i].Square + space);
            }
            Console.Write(Environment.NewLine);
            Console.Write("Mirage: ");
            for (int i = 0; i < results.Item1.Count; i++)
            {
                WriteExpectedString(results.Item1[i].ExpectedTotal);
            }
            Console.Write(Environment.NewLine);
            Console.Write("Oasis:  ");
            for (int i = 0; i < results.Item2.Count; i++)
            {
                WriteExpectedString(results.Item2[i].ExpectedTotal);
            }
            Console.Write(Environment.NewLine);
        }

        public static void PrintTopPlays(List<Tuple<string, double>> topPlays)
        {
            for (int i = 0; i < topPlays.Count; i++)
            {
                if (i < 9)
                {
                    Console.Write(" ");
                }
                Console.Write((i + 1).ToString() + ":");
                WriteExpectedString(topPlays[i].Item2);
                Console.Write(topPlays[i].Item1);
                Console.Write(Environment.NewLine);
            }
        }

        public static void PrintColoursLeft(Dice dice)
        {
            foreach (var colour in dice.CamelsLeft)
            {

                SetBrightPrintColour((Colour)colour);
                Console.Write(((Colour)colour).ToInitialString());
                ResetColour();
            }
            Console.Write(Environment.NewLine);
        }
        
        private static void SetBrightPrintColour(Colour colour)
        {
            switch (colour)
            {
                case Colour.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case Colour.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Colour.White:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Colour.Orange:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Colour.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private static void ResetColour()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void WriteLegBetValue(int number)
        {
            if (number == 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(number);
            }
            if (number == 3)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(number);
            }
            if (number == 2)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(number);
            }
            ResetColour();
        }

        private static void WriteEndBetValue(EndBet endBet)
        {
            var win = endBet.Win == true ? "W" : "L";
            if (endBet.Place < 3)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (endBet.Place < 5)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            Console.Write(win);
            ResetColour();
        }

        private static void WriteExpectedString(double number)
        {
            if (number >= 1.5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else if (number >= 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else if (number >= 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }
            Console.Write(number.ToExpectedString());
            ResetColour();
        }
    }
}
