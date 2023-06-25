using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public static class Collections {
    public static void Fill<T>(T[] arr, T value) {
        System.Array.Fill(arr, value);
    }

    public static void Fill<T>(List<T> arr, T value) {
        for (var i = 0; i < arr.Count; ++i) {
            arr[i] = value;
        }
    }

    public static void Fill<T>(T[,] arr, T value) {
        for (var i = 0; i < arr.GetLength(0); ++i) {
            for (var j = 0; j < arr.GetLength(1); ++j) {
                arr[i,j] = value;
            }
        }
    }

    public static void Fill<T>(T[] arr, System.Func<int, T> value) {
        for (var i = 0; i < arr.Length; ++i) {
            arr[i] = value(i);
        }
    }

    public static void Fill<T>(List<T> arr, System.Func<int, T> value) {
        for (var i = 0; i < arr.Count; ++i) {
            arr[i] = value(i);
        }
    }

    public static List<T> Repeat<T>(T value, int capacity) {
        return Enumerable.Repeat(value, capacity).ToList();
    }

    public static List<T> Repeat<T>(System.Func<T> value, int capacity) {
        List<T> r = new List<T>();
        for (var i = 0; i < capacity; ++i) {
            r.Add(value());
        }
        return r;
    }

    public static List<T> Repeat<T>(System.Func<int, T> value, int capacity) {
        List<T> r = new List<T>();
        for (var i = 0; i < capacity; ++i) {
            r.Add(value(i));
        }
        return r;
    }

    public static T ChooseRandom<T>(T[] collection) {
        return collection[UnityEngine.Random.Range(0, collection.Length)];
    }

    public static T ChooseRandom<T>(IList<T> collection) {
        return collection[UnityEngine.Random.Range(0, collection.Count)];
    }

    public static T ChooseRandom<T>(T[] collection, System.Random rng) {
        if (rng == null) {
            return ChooseRandom(collection);
        }
        return collection[(int)(collection.Length * rng.NextDouble())];
    }

    public static T ChooseRandom<T>(IList<T> collection, System.Random rng) {
        if (rng == null) {
            return ChooseRandom(collection);
        }
        return collection[(int)(collection.Count * rng.NextDouble())];
    }

    public static int RandomIndexMatchingCondition<T>(IList<T> collection, System.Func<T, bool> condition) {
        var idx = -1;
        var numFound = 0;
        for (var i = 0; i < collection.Count; ++i) {
            if (condition(collection[i])) {
                ++numFound;
                if (UnityEngine.Random.Range(0, numFound) == 0) {
                    idx = i;
                }
            }
        }

        return idx;
    }

    public static T ChooseRandomMatchingCondition<T>(IList<T> collection, System.Func<T, bool> condition) {
        var idx = RandomIndexMatchingCondition(collection, condition);
        if (idx == -1) {
            return default(T);
        }
        return collection[idx];
    }

    public static void ShuffleInPlace<T>(T[] arr) {
        for (int t = 0; t < arr.Length; t++) {
            T tmp = arr[t];
            int r = UnityEngine.Random.Range(t, arr.Length);
            arr[t] = arr[r];
            arr[r] = tmp;
        }
    }

    public static void ShuffleInPlace<T>(IList<T> arr) {
        for (int t = 0; t < arr.Count; t++) {
            T tmp = arr[t];
            int r = UnityEngine.Random.Range(t, arr.Count);
            arr[t] = arr[r];
            arr[r] = tmp;
        }
    }

    public static void ShuffleInPlace<T>(T[] arr, System.Random rng) {
        for (int t = 0; t < arr.Length; t++) {
            T tmp = arr[t];
            int r = t + (int)(rng.NextDouble() * (arr.Length - t));
            arr[t] = arr[r];
            arr[r] = tmp;
        }
    }

    public static void ShuffleInPlace<T>(IList<T> arr, System.Random rng) {
        for (int t = 0; t < arr.Count; t++) {
            T tmp = arr[t];
            int r = t + (int)(rng.NextDouble() * (arr.Count - t));
            arr[t] = arr[r];
            arr[r] = tmp;
        }
    }

    /// <summary> Collects the values corresponding to each key into individual lists,
    /// given a list of key-value pairs.</summary>
    public static Dictionary<K, List<V>> SeparateByKey<K, V>(IEnumerable<(K, V)> collection) {
        Dictionary<K, List<V>> d = new Dictionary<K, List<V>>();
        foreach (var c in collection) {
            if (!d.ContainsKey(c.Item1)) {
                d[c.Item1] = new List<V>();
            }
            d[c.Item1].Add(c.Item2);
        }

        return d;
    }

    /// <summary> Performs a `reduce` on each unique key of a set of key-value pairs.</summary>
    public static Dictionary<K, V> ReduceByKey<K, V>(IEnumerable<(K, V)> collection, System.Func<V, V, V> reducer) {
        Dictionary<K, V> d = new Dictionary<K, V>();
        foreach (var c in collection) {
            if (!d.ContainsKey(c.Item1)) {
                d[c.Item1] = c.Item2;
                continue;
            }
            d[c.Item1] = reducer(d[c.Item1], c.Item2);
        }

        return d;
    }

    public static Dictionary<T, int> Counter<T>(IEnumerable<T> collection) {
        return ReduceByKey(collection.Select(i => (i, 1)), (a, b) => a + b);
    }

    /// <summary> Given a collection and a scoring function, returns the element
    /// in the collection with the minimum score, as well as that score.</summary>
    public static Tuple<A, float> MinimumByScoringFn<A>(this IEnumerable<A> candidates, System.Func<A, float> scoreFn) {
        Tuple<A, float> bestSoFar = null;
        foreach (var candidate in candidates) {
            var score = scoreFn(candidate);
            if (bestSoFar == null || score < bestSoFar.Item2) {
                bestSoFar = new Tuple<A, float>(candidate, score);
            }
        }

        return bestSoFar;
    }

    /// <summary>Checks whether two collections contain the same elements,
    /// possibly in a different order.</summary>
    public static bool SameElements<T>(ICollection<T> a, ICollection<T> b) where T : System.IComparable<T> {
        if (a.Count != b.Count) {
            return false;
        }

        var sa = a.ToList();
        sa.Sort();
        var sb = b.ToList();
        sb.Sort();

        for (var i = 0; i < sa.Count; ++i) {
            if (!EqualityComparer<T>.Default.Equals(sa[i], sb[i])) {
                return false;
            }
        }

        return true;
    }
}