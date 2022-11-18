using System;
using System.Collections.Generic;

namespace NijiDive.Utilities
{
    public static class CollectionUtilities
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[n];
                list[k] = temp;
            }
        }
    }
}