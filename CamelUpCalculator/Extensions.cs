using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace CamelUpCalculator
{
    public static class Extensions
    {
        public static List<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            var newList = new List<T>();
            for (int i = 0; i < n; i++)
            {
                newList.Add(list[i]);
            }

            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = newList[k];
                newList[k] = newList[n];
                newList[n] = value;
            }

            return newList;
        }

        public static List<T> CloneList<T>(this IList<T> list)
        {
            var clone = new List<T>();
            var length = list.Count;
            for (int i = 0; i < length; i++)
            {
                clone.Add(list[i]);
            }

            return clone;
        }

        public static T[] CloneArray<T>(this T[] array)
        {
            var length = array.Length;
            var clone = new T[length];
            for (int i = 0; i < length; i++)
            {
                clone[i] = array[i];
            }

            return clone;
        }

        public static void Swap(this int[] array, int firstIndex, int secondIndex)
        {
            var firstValue = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = firstValue;
        }

        public static void AddOrIncrement<T>(this Dictionary<T, double> dictionary, T key, double value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] += value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        public static string ToPercentageString(this double number)
        {
            var percentage = Math.Round(100 * number);
            var space = percentage < 10 ? "  " : (percentage < 100 ? " " : "");
            return space + percentage.ToString() + "%";
        }

        public static string ToExpectedString(this double number)
        {
            var expectation = Math.Round(number, 2).ToString();
            var point = expectation.IndexOf(".");
            if (point == 1)
            {
                expectation = " " + expectation;
            }
            if (point == -1)
            {
                expectation = expectation + "   ";
                for (int i = 0; i < 5 - expectation.Length; i++)
                {
                    expectation = " " + expectation;
                }
            }
            else
            {
                for (int i = 0; i < 5 - expectation.Length; i++)
                {
                    expectation = expectation + " ";
                }
            }

            return expectation;
        }

        public static string ToXMLString(this int number, string initial)
        {
            return initial + number.ToString();
        }

        public static int XMLToInt(this XName xName)
        {
            var name = xName.LocalName;
            name = name.Remove(0, 1);
            return int.Parse(name);
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }

    public static class EnumExtensions
    {
        public static string ToInitialString(this Colour colour)
        {
            switch (colour)
            {
                case Colour.Blue:
                    return "B";
                case Colour.Yellow:
                    return "Y";
                case Colour.White:
                    return "W";
                case Colour.Orange:
                    return "O";
                case Colour.Green:
                    return "G";
                default:
                    throw new ArgumentException();
            }
        }

        public static Colour? ParseColour(this char initial)
        {
            switch (initial)
            {
                case ('b'):
                case ('B'):
                    return Colour.Blue;
                case ('y'):
                case ('Y'):
                    return Colour.Yellow;
                case ('o'):
                case ('O'):
                    return Colour.Orange;
                case ('g'):
                case ('G'):
                    return Colour.Green;
                case ('w'):
                case ('W'):
                    return Colour.White;
                default:
                    return null;
            }
        }

        public static string ToFriendlyString(this Colour colour)
        {
            switch (colour)
            {
                case Colour.Blue:
                    return "Blue  ";
                case Colour.Yellow:
                    return "Yellow";
                case Colour.White:
                    return "White ";
                case Colour.Orange:
                    return "Orange";
                case Colour.Green:
                    return "Green ";
                default:
                    throw new ArgumentException();
            }
        }

        public static string ToFriendlyString(this Modifier modifier)
        {
            switch (modifier)
            {
                case (Modifier.Mirage):
                    return "/";
                case (Modifier.Oasis):
                    return "\\";
                case (Modifier.None):
                    return "_";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
