using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpotRegistry : Registry<HidingSpotRegistry, HidingSpot> {}
public class HidingSpot : MonoBehaviour, Entity<HidingSpotRegistry, HidingSpot>
{
    public GameManager gameManager;
    public HidingspotManager hidingspotManager;
    public int trapValue = 0,collectibleValue = 0;

    public Collectible hiddenItem;

    public Trap trap;

    public int uniqueId {get; set;}
    public HidingSpotRegistry registry {get; set;}
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



}
