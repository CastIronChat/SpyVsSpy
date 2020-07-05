using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public GameManager gameManager;
    public HidingspotManager hidingspotManager;
    public int spotInList = -1;
    public bool canBeBumpedInto;
    public TrapType trapValue;
    public int collectibleValue = 0;

    public Collectible hiddenItem;

    public Trap trap;

    private Animator anim;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Q))
        {PlayAnimation("search");}
    }

    //send ints over the network and compare them to a master list
    public TrapType GetTrap()
    {return trapValue;}
    public int GetCollectible()
    {return collectibleValue;}

    public void SetTrap(TrapType newtrap)
    {
      trapValue = newtrap;

    }
    public void SetCollectible(int newcollectible)
    {
       // collectibleValue = newcollectible; GetComponent<SpriteRenderer>().color = new Vector4(0.1f * newcollectible, 0.9f * newcollectible, 0.6f,1.0f);
     }



    public int GetPlaceInList()
    {return spotInList;}
    //have all hiding spots set on the server so that when interacting each player will check/update through the hiding spot manager, rather than each item.
    public void SetSpotInList(int newplace, GameManager newGM)
    {
        gameManager = newGM;
        spotInList = newplace;
    }

    public void PlayAnimation(string animation)
    {
      if(anim == null){anim = GetComponent<Animator>();}

      anim.Play(animation);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if( canBeBumpedInto == true)
        {
              if(  col.transform.GetComponent<Player>() && GetTrap() != null)
              {
                  gameManager.PlayerBumpHidingSpot(col.transform.GetComponent<PhotonView>().ownerId, spotInList ,GetTrap());
              }
            //  gameManager.SyncPyhsicsLocation(spotInList);
        }

    }

    public void OnCollisionStay2D(Collision2D col)
    {
        if( canBeBumpedInto == true)
        {

              //gameManager.SyncPyhsicsLocation(spotInList);
        }

    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(  canBeBumpedInto && col.transform.GetComponent<Player>() && GetTrap() != null)
        {
          gameManager.PlayerBumpHidingSpot(col.transform.GetComponent<PhotonView>().ownerId, spotInList ,GetTrap());
        }

    }

}
