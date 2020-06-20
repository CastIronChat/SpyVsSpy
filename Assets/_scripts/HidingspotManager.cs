using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingspotManager : MonoBehaviour
{

  public GameManager gameManager;
  public ScrollingText scrollingText;
  public Transform hidingSpotParent;
  public List<HidingSpot> hidingSpots;
    // Start is called before the first frame update
    void Start()
    {
      SetHidingspotList();
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


    public void SetTrapForHidingSpot(int whichspot,int whattrap)
    {
        if(whichspot < hidingSpots.Count)
        {
          hidingSpots[whichspot].SetTrap(whattrap);
        }
    }

    public void SetHidingspotList()
    {

        while ( hidingSpots.Count < hidingSpotParent.childCount )
        {
            Transform closestHidingSpot = hidingSpotParent.GetChild( 0 );
            float dist = Vector3.Distance( closestHidingSpot.position, transform.position );
            foreach ( Transform go in hidingSpotParent )
            {
                if (Vector3.Distance( go.position, transform.position ) < dist)
                {
                    closestHidingSpot = go;
                    dist = Vector3.Distance( go.position, transform.position );
                }
            }


            closestHidingSpot.GetComponent<HidingSpot>().SetSpotInList( hidingSpots.Count, gameManager );

            hidingSpots.Add( closestHidingSpot.GetComponent<HidingSpot>() );
        }

    }
}
