using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public bool doorlistset = false, hidingspotlistset = false;
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
        if(whichspot < hidingSpots.Count && whichspot >= 0)
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
        if (doorlistset == false)
        {
            doorlistset = true;
            var allDoors = new List<Door>(map.getComponents<Door>());
            if (allDoors.Count <= 0)
            { doorlistset = false; return; }
            // Assign all doors IDs based on their position in the scene, like words in a book:
            // sorted top to bottom, left to right.
            allDoors.Sort(HelperMethods.compareByTransform);
            foreach (var door in allDoors)
            {
                doors.add(door);
                door.gameManager = gameManager;
                door.doorManager = this;
            }
        }
    }

    public void SetHidingspotList()
    {
        if (hidingspotlistset == false)
        {
            hidingspotlistset = true;
            var allHidingSpots = new List<HidingSpot>(map.getComponents<HidingSpot>());
            print("allhidingspoits :" + allHidingSpots.Count);
            if (allHidingSpots.Count <= 0)
            { hidingspotlistset = false; return; }
            // Assign all doors IDs based on their position in the scene, like words in a book:
            // sorted top to bottom, left to right.
            allHidingSpots.Sort(HelperMethods.compareByTransform);
            foreach (var hidingSpot in allHidingSpots)
            {
                //if (hidingSpot.transform.gameObject.active == true)
                //{
                    hidingSpot.SetSpotInList(hidingSpots.Count, gameManager);
                    hidingSpots.Add(hidingSpot);
                //}
            }
        }
    }
}
