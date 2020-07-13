using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Placed on the root GameObject of the Map.  This hierarchy must contain all map elements: hiding places, doors, etc.
/// But they can exist in any arrangement. (deeply nested under child objects or not)
/// </summary>
public class Map : MonoBehaviour
{
    public RectTransformUtility roomSizeReference;
    public Transform rooms;

    /// <summary>
    /// At the moment, a silly abstraction, since we're just wrapping GetComponentsInChildren<>
    /// Maybe it will need to do extra filtering in the future; I'm not sure.
    /// </summary>
    public T[] getComponents<T>() where T : Component
    {
        return GetComponentsInChildren<T>();
    }

    private void OnValidate()
    {
        Assert.IsNotNull( roomSizeReference, "Map must have a reference to its roomSizeReference so that cameras know how to snap to rooms." );
    }
}
