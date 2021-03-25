using System;
using System.Collections.Generic;
using System.Linq;

namespace CamelUpCalculator
{
    [Serializable]
    public class BoardState
    {
        public Camel BlueCamel { get; }
        public Camel WhiteCamel { get; }
        public Camel OrangeCamel { get; }
        public Camel GreenCamel { get; }
        public Camel YellowCamel { get; }

        public Square[] Board { get; }

        public bool IsWon { get; private set; }
        public Colour Winner { get; private set; }

        public BoardState()
        {
            Board = new Square[Constants.NumberOfSquares + 1];
            for (int i = 0; i < Constants.NumberOfSquares + 1; i++)
            {
                Board[i] = new Square(i);
            }

            BlueCamel = new Camel(Colour.Blue);
            YellowCamel = new Camel(Colour.Yellow);
            OrangeCamel = new Camel(Colour.Orange);
            WhiteCamel = new Camel(Colour.White);
            GreenCamel = new Camel(Colour.Green);

            IsWon = false;
        }

        public void Reset()
        {
            for (int i = 0; i < Constants.NumberOfSquares + 1; i++)
            {
                Board[i] = new Square(i);
            }

            BlueCamel.Reset();
            YellowCamel.Reset();
            OrangeCamel.Reset();
            WhiteCamel.Reset();
            GreenCamel.Reset();

            ClearModifiers();

            IsWon = false;
        }


        public void MoveCamel(Colour colour, int roll)
        {
            var camel = GetCamel(colour);
            var height = camel.Height;
            var currentSquare = Board[camel.Square];

            var camelsToMove = new List<Colour>();
            if (currentSquare.Index == 0)
            {
                camelsToMove.Add(colour);
            }
            else
            {
                camelsToMove = currentSquare.RemoveCamels(height);
            }

            if (camel.Square + roll >= Board.Length)
            {
                if (!IsWon)
                {
                    Winner = camelsToMove.Last();
                }
                IsWon = true;
                return;
            }

            var newSquare = Board[camel.Square + roll];
            var addBelow= false;
            if (newSquare.Modifier == Modifier.Mirage)
            {
                newSquare = Board[camel.Square + roll - 1];
                addBelow = true;
            }
            else if (newSquare.Modifier == Modifier.Oasis)
            {
                newSquare = Board[camel.Square + roll + 1];
            }

            Update(camelsToMove, newSquare, addBelow);
            newSquare.AddCamels(camelsToMove, addBelow);
        }

        public void MoveCamel(Roll roll)
        {
            MoveCamel((Colour)roll.Camel, roll.Number);
        }

        public Colour GetLeadingCamel()
        {
            for (int i = Board.Length - 1; i >= 0; i--)
            {
                if (Board[i].CamelCount > 0)
                {
                    return Board[i].Camels.Last();
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        public bool PlaceModifier(int square, Modifier modifier)
        {
            if (!AvailableForModifier(square))
            {
                return false;
            }

            return Board[square].PlaceModifier(modifier);
        }

        public void ClearModifiers()
        {
            for (int i = 0; i < Board.Length; i++)
            {
                Board[i].RemoveModifier();
            }
        }

        public List<int> GetAvailableSquares()
        {
            var availableSquares = new List<int>();
            var afterFirstCamel = false;
            for (int i = 1; i <= Constants.NumberOfSquares; i++)
            {
                if (Board[i].CamelCount > 0)
                {
                    afterFirstCamel = true;
                    continue;
                }

                if (afterFirstCamel && AvailableForModifier(i))
                {
                    availableSquares.Add(i);
                }
            }

            return availableSquares;
        }

        public SimulationState GetState()
        {
            var position = new int[Constants.NumberOfCamels];
            var permutation = new int[Constants.NumberOfCamels];
            var modifiers = new List<int>();
            var camel = 0;

            for (int i = Constants.NumberOfSquares; i > 0; i--)
            {
                if (Board[i].Modifier == Modifier.Mirage)
                {
                    modifiers.Add(i * -1);
                }
                else if (Board[i].Modifier == Modifier.Oasis)
                {
                    modifiers.Add(i);
                }
                else
                {
                    for (int j = Board[i].CamelCount; j > 0; j--)
                    {
                        position[camel] = i;
                        var camelColour = Board[i].Camels[j - 1];
                        permutation[(int)camelColour] = camel;
                        camel++;
                    }
                }
            }

            if (position.ToList().Contains(0))
            {
                position = null;
            } 

            return new SimulationState(position, permutation, modifiers);
        }

        private void Update(List<Colour> camels, Square newSquare, bool addBelow)
        {
            var squareCamelHeight = newSquare.CamelCount;

            if (addBelow)
            {
                squareCamelHeight = 0;
                // Move exising camels higher
                for (int i = 0; i < newSquare.CamelCount; i++)
                {
                    GetCamel(newSquare.Camels[i]).Height += camels.Count;
                }
            }

            for (int i = 0; i < camels.Count; i++)
            {
                var camel = GetCamel(camels[i]);
                camel.Square = newSquare.Index;
                camel.Height = squareCamelHeight + i;
            }
        }

        private Camel GetCamel(Colour colour)
        {
            switch (colour)
            {
                case Colour.Blue:
                    return BlueCamel;
                case Colour.Yellow:
                    return YellowCamel;
                case Colour.White:
                    return WhiteCamel;
                case Colour.Orange:
                    return OrangeCamel;
                case Colour.Green:
                    return GreenCamel;
                default:
                    throw new ArgumentException();
            }
        }

        private bool AvailableForModifier(int square)
        {
            if (Board[square].CamelCount > 0)
            {
                return false;
            }
            else if (Board[square].Modifier != Modifier.None)
            {
                return false;
            }
            else if (square + 1 <= Constants.NumberOfSquares && Board[square + 1].Modifier != Modifier.None)
            {
                return false;
            }
            else if (square - 1 > 0 && Board[square - 1].Modifier != Modifier.None)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
