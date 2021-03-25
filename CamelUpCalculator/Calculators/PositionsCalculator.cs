using System;
using System.Collections.Generic;

namespace CamelUpCalculator
{
    public static class PositionsCalculator
    {
        private static readonly int[] _powerArray;

        static PositionsCalculator()
        {
            _powerArray = PowerArray();
            VictoryPosition = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                VictoryPosition[i] = -1;
            }
        }

        public static readonly int[] VictoryPosition;

        public static Dictionary<int, int[]> GetAllPositions()
        {
            var intialPosition = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                intialPosition[i] = 1;
            }

            var positions = new Dictionary<int, int[]>
            {
                { 0, Clone(intialPosition) }
            };

            var finished = false;
            do
            {
                finished = IteratePositions(positions, intialPosition);
            } while (!finished);

            positions.Add(-1, VictoryPosition.CloneArray());

            return positions;
        }

        public static int PositionToInt(int[] position)
        {
            // Check for victory position
            if (position[0] == -1)
            {
                return -1;
            }

            int integer = 0;
            var length = position.Length;
            for (int i = 0; i < length; i++)
            {
                integer += (position[i] - 1) * (_powerArray[i]);
            }
            return integer;
        }

        private static bool IteratePositions(Dictionary<int, int[]> positions, int[] position)
        {
            if (position[0] == Constants.NumberOfSquares)
            {
                if (ShuffleUp(position, 0))
                {
                    return true;
                }
            }
            else
            {
                position[0]++;
            }

            positions.Add(PositionToInt(position), Clone(position));

            return false;
        }

        private static bool ShuffleUp(int[] position, int camel)
        {
            if (camel == Constants.NumberOfCamels - 1)
            {
                return true;
            }
            else if (position[camel + 1] == Constants.NumberOfSquares)
            {
                return ShuffleUp(position, camel + 1);
            }
            else
            {
                position[camel + 1]++;
                for (int i = 0; i < camel + 1; i++)
                {
                    position[i] = position[camel + 1];
                }
            }

            return false;
        }

        private static int[] Clone(int[] position)
        {
            var clone = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                clone[i] = position[i];
            }
            return clone;
        }

        private static int[] PowerArray()
        {
            var array = new int[Constants.NumberOfCamels];
            for (int i = 0; i < Constants.NumberOfCamels; i++)
            {
                array[i] = (int)Math.Pow(Constants.NumberOfSquares, i);
            }
            return array;
        }
    }
}


