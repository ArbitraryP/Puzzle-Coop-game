using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShuffleIntArray
{
    public static int[] Shuffe(int[] array)
    {
        int length = array.Length;
        int rng = Random.Range(0, length - 1);

        for(int i = 0; i < length; i++)
        {
            Swap(array, i, rng);
        }

        return array;
    }

    private static void Swap(int[] array, int a, int b)
    {
        int temp = array[a];
        array[a] = array[b];
        array[b] = temp;
    }
}
