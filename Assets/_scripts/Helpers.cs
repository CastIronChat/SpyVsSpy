using System.Collections.Generic;
using UnityEngine;

public static class Helpers {
    public static IEnumerator<Transform> getChildTransformEnumerator(this Transform t) {
        foreach(Transform child in t) {
            yield return child;
        }
    }
    public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
    {
      while(enumerator.MoveNext())
          yield return enumerator.Current;
    }
}
