using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingspotManager : MonoBehaviour
{

  public GameManager gameManager;
  public ScrollingText scrollingText;
  public Transform hidingSpotParent,doorsParent,activeHidingSpots;
  public List<HidingSpot> hidingSpots;
  public List<Door> doors;
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

    public void SetTrapForHidingSpot(int whichspot,int whattrap)
    {
        if(whichspot < hidingSpots.Count)
        {
          hidingSpots[whichspot].SetTrap(whattrap);
        }
    }

    public void SetDoorList()
    {
      foreach ( Transform go in doorsParent )
      {
        go.GetComponent<Door>().SetSpotInList( doors.Count,gameManager );
        doors.Add( go.GetComponent<Door>() );
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
          //  }

        }

    }
}
