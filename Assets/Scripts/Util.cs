using System;

public static class Settings
{
    public static int gridSize = 1;


}

public static class Util
{
    public static void Shuffle(object[] arr)
    {
        Random rand = new Random();
        for (int i = arr.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);
            object tmp = arr[j];
            arr[j] = arr[i];
            arr[i] = tmp;
        }
    }
}

public enum EdgeType
{
    Field,
    Road,
    City
}