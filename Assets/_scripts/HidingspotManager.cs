using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HidingspotManager : MonoBehaviour
{

  public GameManager gameManager;
  public ScrollingText scrollingText;
  public Transform hidingSpotParent, doorsParent;
  public HidingSpotRegistry hidingSpots = new HidingSpotRegistry();
  public DoorRegistry doors;
    // Start is called before the first frame update
    void Start()
    {
        doors = new DoorRegistry(gameManager);
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
        if(hidingSpots.hasEntity(whichspot)) {
            hidingSpots.getEntity(whichspot).SetCollectible(whatitem);
        }
    }

    public HidingSpot GetHidingSpot(int whichspot)
    {
        if(hidingSpots.hasEntity(whichspot)) {
        return hidingSpots.getEntity(whichspot);
        }
        return null;
    }

    public void OpenDoor(int whichdoor,bool open)
    {
        if(doors.hasEntity(whichdoor))
      {
         doors[whichdoor].SetOpen(open);
      }
    }

    public void SetTrapForHidingSpot(int whichspot,int whattrap)
    {
        if(hidingSpots.hasEntity(whichspot))
        {
          hidingSpots[whichspot].SetTrap(whattrap);
        }
    }

    public void SetDoorList()
    {
      foreach ( Transform go in doorsParent )
      {
        doors.addEntity(go.GetComponent<Door>());
      }

        // while ( doors.Count < doorsParent.childCount )
        // {
        //     Transform closestDoor =  doorsParent.GetChild( 0 );
        //     float dist = Vector3.Distance( closestDoor.position, transform.position );
        //     foreach ( Transform go in doorsParent )
        //     {
        //         if (closestDoor.GetComponent<Door>().spotInList != -1 || Vector3.Distance( go.transform.position, transform.position ) < dist  )
        //         {
        //             closestDoor = go;
        //             dist = Vector3.Distance( go.transform.position, transform.position );
        //         }
        //     }
        //
        //
        //     closestDoor.GetComponent<Door>().SetSpotInList( doors.Count,gameManager );
        //
        //     doors.Add( closestDoor.GetComponent<Door>() );
        // }

    }

    public void SetHidingspotList()
    {
        var hidingSpotTransforms = hidingSpotParent.getChildTransformEnumerator().ToEnumerable().ToArray();
        var sortedHidingSpotTransforms =
            hidingSpotTransforms.OrderBy(hidingSpotTransform => Vector3.Distance(hidingSpotTransform.position, transform.position));
        foreach(var hidingSpotTransform in sortedHidingSpotTransforms) {
            hidingSpots.addEntity(hidingSpotTransform.GetComponent<HidingSpot>());
        }
    }
}
