using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleIntArray
{
    public static void Shuffe(int[] array)
    {
        int length = array.Length;
        System.Random rng = new System.Random();

        for(int i = 0; i < length; i++)
        {
            Swap(array, i, i + rng.Next(length - i));
        }
    }

    private static void Swap(int[] array, int a, int b)
    {
        int temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
}
