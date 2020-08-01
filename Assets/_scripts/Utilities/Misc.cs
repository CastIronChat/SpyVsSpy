using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif

public static class HelperMethods
{
    ///<summary>
    /// Get a value from a dictionary, or return the default value (usually null) if the dictionary doesn't have it.
    ///</summary>
    public static V GetOrDefault<K, V>(this Dictionary<K, V> dictionary, K key)
    {
        V value;
        if ( dictionary.TryGetValue( key, out value ) )
        {
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

    public static TAccumulate Aggregate<TSource, TAccumulate>(
        this System.Collections.Generic.IEnumerable<TSource> source, TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        TAccumulate r = seed;
        foreach ( var value in source )
        {
            r = func( r, value );
        }

        return r;
    }

    public static Rect ToScreenSpace(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale( transform.rect.size, transform.lossyScale );
        return new Rect( (Vector2) transform.position - (size * 0.5f), size );
    }

#if UNITY_EDITOR
    public static bool IsPrefabStage(this GameObject gameObject)
    {
        return PrefabStageUtility.GetCurrentPrefabStage() != null;
    }

    public static bool IsPrefabStage(this Component component)
    {
        return component.gameObject.IsPrefabStage();
    }

    public static bool IsRootPrefabStage(this GameObject gameObject)
    {
        // return PrefabStageUtility.GetCurrentPrefabStage() != null && !PrefabUtility.IsPartOfPrefabInstance(gameObject);
        return PrefabStageUtility.GetCurrentPrefabStage() != null && !PrefabUtility.IsPartOfAnyPrefab( gameObject );
    }

    public static bool IsRootPrefabStage(this Component component)
    {
        return component.gameObject.IsRootPrefabStage();
    }
#endif

    /// <summary>
    /// Comparer to sort Components by their global world position.
    /// Slow but acceptable to reliably order a list of items in a scene.
    ///
    /// Sorts by y, then x, then z.  In 2D, this sorts like words in a book:
    /// top to bottom, left to right.
    /// </summary>
    public static int compareByTransform(Component a, Component b)
    {
        return compareByTransform( a.transform, b.transform );
    }

    public static int compareByTransform(Transform ta, Transform tb)
    {
        var tap = ta.position;
        var tbp = tb.position;
        if ( tap.y != tbp.y ) return tap.y < tbp.y ? -1 : 1;
        if ( tap.x != tbp.x ) return tap.x < tbp.x ? -1 : 1;
        if ( tap.z != tbp.z ) return tap.z < tbp.z ? -1 : 1;
        return 0;
    }
}
