using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Utils
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
        {
            var original = self.ToList();
            while (original.Any())
            {
                var selectedIndex = Random.Range(0, original.Count);
                yield return original[selectedIndex];
                original.RemoveAt(selectedIndex);
            }
        }
    }
}