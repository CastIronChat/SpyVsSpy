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
    public Transform rooms,hazards,enviroment;
    public MapUi mapui;
    /// <summary>
    /// At the moment, a silly abstraction, since we're just wrapping GetComponentsInChildren<>
    /// Maybe it will need to do extra filtering in the future; I'm not sure.
    /// </summary>
    public T[] getComponents<T>(bool includeInactive = false) where T : Component
    {
        return GetComponentsInChildren<T>(includeInactive);
    }

    private void OnValidate()
    {
        Assert.IsNotNull( roomSizeReference, "Map must have a reference to its roomSizeReference so that cameras know how to snap to rooms." );
    }

    public void MarkRoomAsVisited(Vector3 location)
    {
        Transform closestRoom = rooms.GetChild(0);
        foreach (Transform room in rooms)
        {
            if (Vector3.Distance(room.position, location) < Vector3.Distance(closestRoom.position, location))
            { closestRoom = room; }
        }
        closestRoom.GetComponent<Room>().SetVisited(true);

    }

    public void IncrementRoomCollectibles(Vector3 location, int increment)
    {
        Transform closestRoom = rooms.GetChild(0);
        foreach (Transform room in rooms)
        {
            if (Vector3.Distance(room.position, location) < Vector3.Distance(closestRoom.position, location))
            { closestRoom = room; }
        }
        closestRoom.GetComponent<Room>().ChangeCollectible(increment);

    }

    public void UpdateMapUi()
    {
        if (mapui == null)
        {
            GameObject findmap = GameObject.Find("Mapui");
            if (findmap != null && findmap.GetComponent<MapUi>() != null) { mapui = findmap.GetComponent<MapUi>(); }

        }
        if (mapui != null)
        {
            int count = 0;
            while (count < rooms.childCount)
            {
                mapui.SetRoomImage(count, rooms.GetChild(count).GetComponent<Room>());
                count++;
            }
        }
    }

    public void OpenMap()
    {

    }

}
