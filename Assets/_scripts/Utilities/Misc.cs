using System;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods {
    ///<summary>
    /// Get a value from a dictionary, or return the default value (usually null) if the dictionary doesn't have it.
    ///</summary>
    public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key) {
        V value;
        if(dictionary.TryGetValue(key, out value)) {
            return value;
        }
        return default(V);
    }
    public static IEnumerator<Transform> getChildTransformEnumerator(this Transform t)
    {
        foreach ( Transform child in t )
        {
            yield return child;
        }
    }
    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
        while ( enumerator.MoveNext() )
            yield return enumerator.Current;
    }

    public static TAccumulate Aggregate<TSource,TAccumulate> (this System.Collections.Generic.IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate,TSource,TAccumulate> func) {
        TAccumulate r = seed;
        foreach(var value in source) {
            r = func(r, value);
        }
        return r;
    }
    public static Rect ToScreenSpace(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        return new Rect((Vector2)transform.position - (size * 0.5f), size);
    }
}
