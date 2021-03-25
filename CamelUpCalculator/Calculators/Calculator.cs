using System;

namespace CamelUpCalculator
{
    public static class Calculator
    {
        public static int Factorial(int number)
        {
            if (number == 1)
            {
                return 1;
            }

            return number * Factorial(number - 1);
        }

        public static int NumberOrPossibleRolls()
        {
            return (int)Math.Pow(3, Constants.NumberOfCamels) * Factorial(Constants.NumberOfCamels);
        }

        public static int NumberOfPossibleRolls(int camels)
        {
            return (int)Math.Pow(3, camels) * Factorial(camels);
        }
    }
}
