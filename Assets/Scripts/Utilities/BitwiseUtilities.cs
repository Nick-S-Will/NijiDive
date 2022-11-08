using UnityEngine;

namespace NijiDive.Utilities
{
    public static class BitwiseUtilities
    {
        public static int BitCount(int n)
        {
            var count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }

            return count;
        }
    }
}