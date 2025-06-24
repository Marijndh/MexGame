using System;
using System.Collections.Generic;

public static class UtilsManager {
    internal static void ShuffleList<T>(List<T> list)
    {
        Random random = new Random();
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            // Generate a random index within the range [i, n)
            int randomIndex = random.Next(i, n);

            // Swap the elements at index i and randomIndex
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}