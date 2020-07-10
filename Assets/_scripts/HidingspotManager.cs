using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingspotManager : MonoBehaviour
{

  public GameManager gameManager;
  public ScrollingText scrollingText;
  public Transform hidingSpotParent,doorsParent,activeHidingSpots;
  public List<HidingSpot> hidingSpots;
  public DoorRegistry doors;
    // Start is called before the first frame update
    void Start()
    {
      SetHidingspotList();
      SetDoorList();
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
        var allDoors = new List<Door>(doorsParent.GetComponentsInChildren<Door>());
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

        while ( 0 < hidingSpotParent.childCount )
        {
            Transform closestHidingSpot = hidingSpotParent.GetChild( 0 );
            float dist = Vector3.Distance( closestHidingSpot.position, transform.position );
            foreach ( Transform go in hidingSpotParent )
            {
                if (Vector3.Distance( go.position, transform.position ) <= dist || closestHidingSpot.GetComponent<HidingSpot>().spotInList != -1)
                {
                    closestHidingSpot = go;
                    dist = Vector3.Distance( go.position, transform.position );
                }
            }

          //  if(closestHidingSpot.GetComponent<HidingSpot>().spotInList == -1)
          //  {
              closestHidingSpot.GetComponent<HidingSpot>().SetSpotInList( hidingSpots.Count, gameManager );

              hidingSpots.Add( closestHidingSpot.GetComponent<HidingSpot>() );
              closestHidingSpot.parent = activeHidingSpots;
              // if(PhotonNetwork.isMasterClient == false)
              // {
              //   closestHidingSpot.GetComponent<Rigidbody2D>().isKinematic = true;
              // }

          //  }

        }

    }
}
