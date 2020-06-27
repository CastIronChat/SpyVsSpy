using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorRegistry : Registry<DoorRegistry, Door> {
    public DoorRegistry(GameManager gameManager) {
        this.gameManager = gameManager;
    }
    public GameManager gameManager;
}

public class Door : MonoBehaviour, Entity<DoorRegistry, Door>
{
  public GameManager gameManager {
      get {
          return registry.gameManager;
      }
  }
    public Transform oppositeDoor;
    public GameObject doorSprite;
    public Vector3 openDirection; //leftright or updown
    public float roomsize;

    public int uniqueId { get; set; }
    public DoorRegistry registry { get; set; }

    // Start is called before the first frame update
    void Start()
    {
      if(doorSprite == null){doorSprite = transform.GetChild(0).gameObject;}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOpen(bool open)
    {
      GetComponent<Collider2D>().isTrigger = open;
      doorSprite.active = !open;
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null )
        {
          // col.transform.position = oppositeDoor.position;
          // GetComponent<Collider2D>().isTrigger = false;
          //   doorSprite.active = true;
                if(col.GetComponent<Player>().cam != null){

                    Transform cam = col.GetComponent<Player>().cam.transform;

                    if( Vector3.Distance(col.transform.position,transform.position + (0.1f * openDirection)) < Vector3.Distance(col.transform.position,transform.position ))
                    {

                        cam.position = new Vector3(transform.position.x,transform.position.y,cam.position.z) + (openDirection * roomsize );
                    }else
                    {
                      cam.position = new Vector3(transform.position.x,transform.position.y,cam.position.z) - (openDirection * roomsize );
                    }
                    gameManager.photonView.RPC( "OpenDoor", PhotonTargets.AllBufferedViaServer, uniqueId, false);
              }
        }

    }


}
