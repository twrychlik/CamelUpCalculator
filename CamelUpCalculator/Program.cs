using System;

namespace CamelUpCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameState = new GameState();
            var positionSimulator = new PositionSimulator();

            Printer.PrintBoard(gameState);

            while (true)
            {
                var arg = Console.ReadLine();
                var inputs = arg.Split();
                if (inputs[0].ToLower() == "exit")
                {
                    // "exit" exits
                    return;
                }
                else if (arg == "")
                {

                }
                else if (inputs[0].ToLower() == "help")
                {
                    ConsoleCommands.Help();
                }
                else if (arg.Length == 2)
                {
                    //b3, y2, w1 moves camels
                    ConsoleCommands.MakeMove(arg, gameState);
                }
                else if (inputs[0].ToLower() == "rollset")
                {
                    //"rollset" rolls and moves a full leg
                    ConsoleCommands.RollSet(gameState);
                }
                else if (inputs[0].ToLower() == "roll")
                {
                    //"roll" rolls and moves a random valid dice
                    ConsoleCommands.Roll(gameState);
                }
                else if (inputs.Length == 2 && inputs[0].ToLower() == "leg" && inputs[1].ToLower() == "result")
                {
                    // "leg" prints out probabilities of the leader for the leg
                    ConsoleCommands.LegProbability(positionSimulator, gameState.BoardState, gameState.Dice);
                }
                else if (inputs.Length == 2 && inputs[0].ToLower() == "end" && inputs[1].ToLower() == "result")
                { 
                    // "end" prints out probabilities for the overall winner and loser
                    ConsoleCommands.EndProbability(positionSimulator, gameState.BoardState, gameState.Dice);
                }
                else if (inputs[0].ToLower() == "all")
                {
                    // "all" prints out all probabiliites
                    ConsoleCommands.AllProbabilities(positionSimulator, gameState);
                }
                else if (inputs[0].ToLower() == "modifiers")
                {
                    // prints out modifier probabilities
                    ConsoleCommands.ModifierProbability(positionSimulator, gameState);
                }
                else if (inputs[0].ToLower() == "mirage" && inputs.Length == 2)
                {
                    // mirage 5 places a mirage on square 5
                    ConsoleCommands.PlaceMirage(inputs[1], gameState);
                }
                else if (inputs[0].ToLower() == "oasis" && inputs.Length == 2)
                {
                    // mirage 5 places an oasis on square 5
                    ConsoleCommands.PlaceOasis(inputs[1], gameState);
                }
                else if (inputs[0].ToLower() == "leg" && inputs.Length == 2)
                {
                    // leg y3 makes a bet on y winning the leg with payoff 3
                    ConsoleCommands.PlaceLegBet(inputs[1], gameState);
                }
                else if (inputs[0].ToLower() == "win" || inputs[0].ToLower() == "lose" && inputs.Length == 2)
                {
                    // win y3 makes the third bet on y winning
                    ConsoleCommands.PlaceEndBet(inputs[0], inputs[1], gameState);
                }
                else if (inputs.Length == 2 && inputs[0].ToLower() == "top")
                {
                   // Displays top 10 plays
                   ConsoleCommands.GetTopPlays(positionSimulator, gameState, inputs[1]);
                }
                else if (arg.ToLower() == "reset")
                {
                    // "reset" starts again
                    ConsoleCommands.Reset(gameState);
                }
                else
                {
                    Printer.Nope();
                }
            }
        }
    }
}
