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
}
