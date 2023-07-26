using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JadePhoenix.Tools
{
    public static class ListExtensions
    {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            var temporary = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = temporary;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0;  i < list.Count; i++) 
            {
                list.Swap(i, Random.Range(i, list.Count));
            }
        }
    }
}
