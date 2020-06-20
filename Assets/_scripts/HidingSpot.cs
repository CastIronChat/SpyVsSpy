using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public GameManager gameManager;
    public HidingspotManager hidingspotManager;
    public int spotInList = -1,trapValue = 0,collectibleValue = 0;

    public Collectible hiddenItem;

    public Trap trap;

    void Update()
    {
      if(Input.GetKeyDown(KeyCode.P))
      {SetTrap(1);}
        if(Input.GetKeyDown(KeyCode.Q))
        {SetCollectible(1);}
    }

    //send ints over the network and compare them to a master list
    public int GetTrap()
    {return trapValue;}
    public int GetCollectible()
    {return collectibleValue;}

    public void SetTrap(int newtrap)
    { trapValue = newtrap; GetComponent<SpriteRenderer>().color = new Vector4(0.9f * newtrap, 0.1f * newtrap, 0.6f,1.0f);}
    public void SetCollectible(int newcollectible)
    { collectibleValue = newcollectible; GetComponent<SpriteRenderer>().color = new Vector4(0.1f * newcollectible, 0.9f * newcollectible, 0.6f,1.0f);}



    public int GetPlaceInList()
    {return spotInList;}
    //have all hiding spots set on the server so that when interacting each player will check/update through the hiding spot manager, rather than each item.
    public void SetSpotInList(int newplace, GameManager newGM)
    {
        gameManager = newGM;
        spotInList = newplace;
    }
}
