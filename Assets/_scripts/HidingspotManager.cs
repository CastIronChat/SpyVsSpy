using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingspotManager : MonoBehaviour
{

    public GameManager gameManager
    {
        get => GameManager.instance;
    }
  public Map map
  {
      get => gameManager.map;
  }
  public ScrollingText scrollingText;
  public List<HidingSpot> hidingSpots;
  public DoorRegistry doors = new DoorRegistry();
    // Start is called before the first frame update
    void Start()
    {
      SetHidingspotList();
      SetDoorList();
      doors.installAsSerializer();
    }
    void OnEnable()
    {
      // SetHidingspotList();
      // SetDoorList();
    }
    // Update is called once per frame
    void Update()
    {

    }


    public void SetCollectibleForHidingSpot(int whichspot,int whatitem)
    {
        if(whichspot < hidingSpots.Count)
        {
          hidingSpots[whichspot].SetCollectible(whatitem);
        }
    }

    public HidingSpot GetHidingSpot(int whichspot)
    {
        if(whichspot < hidingSpots.Count && whichspot >= 0)
        {
          return hidingSpots[whichspot];
        }
        return null;
    }

    public void OpenDoor(int whichdoor,bool open)
    {
      if(whichdoor < doors.Count)
      {
         doors[whichdoor].SetOpen(open);
      }
    }

    public void SetTrapForHidingSpot(int whichspot, TrapType trapType)
    {
        if(whichspot < hidingSpots.Count)
        {
          hidingSpots[whichspot].SetTrap(trapType);
        }
    }

    public void SetDoorList()
    {
        var allDoors = new List<Door>(map.getComponents<Door>());
        // Assign all doors IDs based on their position in the scene, like words in a book:
        // sorted top to bottom, left to right.
        allDoors.Sort(HelperMethods.compareByTransform);
        foreach ( var door in allDoors )
        {
            doors.add(door);
            door.gameManager = gameManager;
            door.doorManager = this;
        }
    }

    public void SetHidingspotList()
    {
        var allHidingSpots = new List<HidingSpot>(map.getComponents<HidingSpot>());
        // Assign all doors IDs based on their position in the scene, like words in a book:
        // sorted top to bottom, left to right.
        allHidingSpots.Sort(HelperMethods.compareByTransform);
        foreach ( var hidingSpot in allHidingSpots )
        {
            hidingSpot.SetSpotInList( hidingSpots.Count, gameManager );
            hidingSpots.Add(hidingSpot);
        }
    }
}
